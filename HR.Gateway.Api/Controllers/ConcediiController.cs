using HR.Gateway.Api.Contracts.Angajati;
using HR.Gateway.Api.Contracts.Concedii.Common;
using HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;
using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Infrastructure.Angajati.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/concedii")]
[Authorize]
public class ConcediiController(
    IVemAngajatiClient vem,
    IConcediiCalculator calculator) : ControllerBase
{
    // GET /api/concedii/cereri/me  -> ia emailul din tokenul curent
    [HttpGet("cereri/me")]
    [ProducesResponseType(typeof(CereriConcediuOdihnaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CereriConcediuOdihnaResponse>> GetCereriMe(CancellationToken ct)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var r = await vem.GetHolidayRequestsByEmailAsync(email, ct);
        if (r is null) return NotFound();

        var angajat = r.Angajat is null ? null : new ProfilAngajatDto
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
            .Select(c => new CerereConcediuOdihnaDto
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

        return Ok(new CereriConcediuOdihnaResponse
        {
            Gasit  = r.Gasit,
            Angajat = angajat,
            Cereri  = cereri
        });
    }

    // POST /api/concedii/cereri  -> pentru admin/servicii: aduce cererile unui email specific
    [HttpPost("cereri")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(CereriConcediuOdihnaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CereriConcediuOdihnaResponse>> GetCereriByEmail(
        [FromBody] CereriConcediuOdihnaRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest("Email lipsă.");

        var r = await vem.GetHolidayRequestsByEmailAsync(req.Email, ct);
        if (r is null) return NotFound();

        var angajat = r.Angajat is null ? null : new ProfilAngajatDto
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
            .Select(c => new CerereConcediuOdihnaDto
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

        return Ok(new CereriConcediuOdihnaResponse
        {
            Gasit  = r.Gasit,
            Angajat = angajat,
            Cereri  = cereri
        });
    }
    
    // POST /api/concedii/calculeaza-zile
    [HttpPost("calculeaza-zile")]
    [Authorize]
    public async Task<ActionResult<CalculeazaZileResponse>> CalculeazaZile(
        [FromBody] CalculeazaZileRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest("Email required.");
        if (req.DataSfarsit < req.DataInceput)
            return BadRequest("DataSfarsit < DataInceput.");

        var start = DateOnly.FromDateTime(req.DataInceput);
        var end   = DateOnly.FromDateTime(req.DataSfarsit);

        var result = await calculator.CalculeazaZileAsync(req.Email, start, end, ct);

        var detaliat = result.PeAnDetaliat
            .Select(x => new ConcediuPeAnDetaliatDto
            {
                An        = x.An,
                AnId      = x.AnId,
                Alocate   = x.Alocate,
                Consumate = x.Consumate,
                Ramase    = x.Ramase
            })
            .ToList();

        return Ok(new CalculeazaZileResponse
        {
            NumarZile     = result.NumarZile,
            PeAnDetaliat  = detaliat
        });
    }

}
