using HR.Gateway.Api.Contracts.Angajati;
using HR.Gateway.Api.Contracts.Concedii.Common;
using HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;
using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Infrastructure.Angajat.Client;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Cereri;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/concedii")]
[Authorize]
public class ConcediiController(
    IVemAngajatClient vem,
    IConcediuCalculator calculator) : ControllerBase
{
    // GET /api/concedii/cereri/me  -> ia emailul din tokenul curent
    [HttpGet("cereri/me")]
    [ProducesResponseType(typeof(CerereConcediuOdihnaGetByEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CerereConcediuOdihnaGetByEmailResponse>> GetCereriMe(CancellationToken ct)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var r = await vem.GetHolidayRequestsByEmailAsync(email, ct);
        if (r is null) return NotFound();

        var angajat = r.Angajat is null ? null : new AngajatProfileResponse
        {
            Nume               = r.Angajat.Nume ?? "",
            Prenume            = r.Angajat.Prenume ?? "",
            NumeComplet        = r.Angajat.NumeComplet ?? $"{r.Angajat.Prenume} {r.Angajat.Nume}",
            Email              = r.Angajat.Email ?? email,
            Departament        = r.Angajat.Departament ?? "",
            Functie            = r.Angajat.Functie ?? "",
            ZileConcediuRamase = r.Angajat.ZileConcediuRamase ?? 0
        };

        var cereri = (r.Cereri ?? new())
            .OrderByDescending(c => c.DataCerere ?? DateTime.MinValue)
            .Select(c => new CerereConcediuOdihnaGetByIdResponse
            {
                Id                = c.Id,
                DataCerere        = c.DataCerere,
                DataInceput       = c.DataInceput,
                DataSfarsit       = c.DataSfarsit,
                NumarZile         = c.NumarZile,
                Inlocuitor        = c.Inlocuitor ?? "",
                NumarInregistrare = c.NumarInregistrare ?? "",
                DataInregistrare  = c.DataInregistrare,
                Stare             = c.Stare ?? ""
            })
            .ToList();

        return Ok(new CerereConcediuOdihnaGetByEmailResponse
        {
            Gasit  = r.Gasit,
            Angajat = angajat,
            Cereri  = cereri
        });
    }

    [HttpGet("cereri/fara-plata/me")]
    [ProducesResponseType(typeof(CerereConcediuOdihnaGetByEmailResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CerereConcediuOdihnaGetByEmailResponse>> GetCereriFaraPlataMe(CancellationToken ct)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var r = await vem.GetUnpaidLeaveRequestsByEmailAsync(email, ct);
        if (r is null) return NotFound();

        return Ok(MapToResponse(r, email));
    }

    [HttpGet("cereri/eveniment/me")]
    [ProducesResponseType(typeof(CerereConcediuOdihnaGetByEmailResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CerereConcediuOdihnaGetByEmailResponse>> GetCereriEvenimentMe(CancellationToken ct)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var r = await vem.GetSpecialEventLeaveRequestsByEmailAsync(email, ct);
        if (r is null) return NotFound();

        return Ok(MapToResponse(r, email));
    }

    private CerereConcediuOdihnaGetByEmailResponse MapToResponse(VemRaspunsCereriConcediuOdihna r, string email)
    {
        var angajat = r.Angajat is null ? null : new AngajatProfileResponse
        {
            Nume               = r.Angajat.Nume ?? "",
            Prenume            = r.Angajat.Prenume ?? "",
            NumeComplet        = r.Angajat.NumeComplet ?? (r.Angajat.Prenume + " " + r.Angajat.Nume),
            Email              = r.Angajat.Email ?? email,
            Departament        = r.Angajat.Departament ?? "",
            Functie            = r.Angajat.Functie ?? "",
            ZileConcediuRamase = r.Angajat.ZileConcediuRamase ?? 0
        };

        var cereri = (GetCereri(r) ?? new List<VemCerereConcediuOdihna>())
            .OrderByDescending(c => c.DataCerere ?? DateTime.MinValue)
            .Select(c => new CerereConcediuOdihnaGetByIdResponse
            {
                Id                = c.Id,
                DataCerere        = c.DataCerere,
                DataInceput       = c.DataInceput,
                DataSfarsit       = c.DataSfarsit,
                NumarZile         = c.NumarZile ?? 0,
                Inlocuitor        = c.Inlocuitor ?? "",
                NumarInregistrare = c.NumarInregistrare ?? "",
                DataInregistrare  = c.DataInregistrare,
                Stare             = c.Stare ?? ""
            })
            .ToList();

        return new CerereConcediuOdihnaGetByEmailResponse
        {
            Gasit  = r.Gasit,
            Angajat = angajat,
            Cereri  = cereri
        };
    }

    private static List<VemCerereConcediuOdihna>? GetCereri(VemRaspunsCereriConcediuOdihna r)
    {
        return r.Cereri ?? r.CereriFaraPlata ?? r.CereriEveniment;
    }

    // POST /api/concedii/cereri  -> pentru admin/servicii: aduce cererile unui email specific
    [HttpPost("cereri")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(CerereConcediuOdihnaGetByEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CerereConcediuOdihnaGetByEmailResponse>> GetCereriByEmail(
        [FromBody] CerereConcediuOdihnaGetByEmailRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest("Email lipsă.");

        var r = await vem.GetHolidayRequestsByEmailAsync(req.Email, ct);
        if (r is null) return NotFound();

        var angajat = r.Angajat is null ? null : new AngajatProfileResponse
        {
            Nume               = r.Angajat.Nume ?? "",
            Prenume            = r.Angajat.Prenume ?? "",
            NumeComplet        = r.Angajat.NumeComplet ?? $"{r.Angajat.Prenume} {r.Angajat.Nume}",
            Email              = r.Angajat.Email ?? req.Email,
            Departament        = r.Angajat.Departament ?? "",
            Functie            = r.Angajat.Functie ?? "",
            ZileConcediuRamase = r.Angajat.ZileConcediuRamase ?? 0
        };

        var cereri = (r.Cereri ?? new())
            .OrderByDescending(c => c.DataCerere ?? DateTime.MinValue)
            .Select(c => new CerereConcediuOdihnaGetByIdResponse
            {
                Id                = c.Id,
                DataCerere        = c.DataCerere,
                DataInceput       = c.DataInceput,
                DataSfarsit       = c.DataSfarsit,
                NumarZile         = c.NumarZile,
                Inlocuitor        = c.Inlocuitor ?? "",
                NumarInregistrare = c.NumarInregistrare ?? "",
                DataInregistrare  = c.DataInregistrare,
                Stare             = c.Stare ?? ""
            })
            .ToList();

        return Ok(new CerereConcediuOdihnaGetByEmailResponse
        {
            Gasit  = r.Gasit,
            Angajat = angajat,
            Cereri  = cereri
        });
    }
    
    // POST /api/concedii/calculeaza-zile
    [HttpPost("calculeaza-zile")]
    [Authorize]
    public async Task<ActionResult<ConcediuCalculateDaysResponse>> CalculeazaZile(
        [FromBody] ConcediuCalculateDaysRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest("Email required.");
        if (req.DataSfarsit < req.DataInceput)
            return BadRequest("DataSfarsit < DataInceput.");

        var start = DateOnly.FromDateTime(req.DataInceput);
        var end   = DateOnly.FromDateTime(req.DataSfarsit);

        var result = await calculator.CalculeazaZileAsync(req.Email, start, end, ct);

        var detaliat = result.PeAnDetaliat
            .Select(x => new ConcediuYearDetailItem
            {
                An        = x.An,
                AnId      = x.AnId,
                Alocate   = x.Alocate,
                Consumate = x.Consumate,
                Ramase    = x.Ramase
            })
            .ToList();

        return Ok(new ConcediuCalculateDaysResponse
        {
            NumarZile     = result.NumarZile,
            PeAnDetaliat  = detaliat
        });
    }

}

