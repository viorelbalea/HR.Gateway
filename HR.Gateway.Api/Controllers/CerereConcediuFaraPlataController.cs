using HR.Gateway.Api.Contracts.Concedii.ConcediuFaraPlata;
using HR.Gateway.Application.Abstractions.CerereConcediu;
using HR.Gateway.Application.Abstractions.CerereConcediuFaraPlata;
using HR.Gateway.Application.Abstractions.Concedii;
using ApplicationCerereFaraPlata = HR.Gateway.Application.Models.CerereConcediuFaraPlata;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/cereri-concedii/fara-plata")]
[Authorize]
public sealed class CerereConcediuFaraPlataController(
    ICerereConcediuFaraPlataWriter writer,
    ICerereConcediuFaraPlataReader reader,
    IConcediuCalculator calculator,
    IInlocuitoriProvider inlocuitori,
    UserManager<AppUser> users
) : ControllerBase
{
    private readonly IInlocuitoriProvider _inlocuitori = inlocuitori;

    // POST api/cereri-concedii/fara-plata/calculeaza-zile
    [HttpPost("calculeaza-zile")]
    [Authorize]
    public async Task<ActionResult<HR.Gateway.Api.Contracts.Concedii.Common.ConcediuCalculateDaysResponse>> CalculeazaZileAsync(
        [FromBody] HR.Gateway.Api.Contracts.Concedii.Common.ConcediuCalculateDaysRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest("Email required.");
        if (req.DataSfarsit < req.DataInceput)
            return BadRequest("DataSfarsit < DataInceput.");

        var start = DateOnly.FromDateTime(req.DataInceput);
        var end = DateOnly.FromDateTime(req.DataSfarsit);

        var result = await calculator.CalculeazaZileAsync(req.Email, start, end, ct);

        var detaliat = result.PeAnDetaliat
            .Select(x => new HR.Gateway.Api.Contracts.Concedii.Common.ConcediuYearDetailItem
            {
                An = x.An,
                AnId = x.AnId,
                Alocate = x.Alocate,
                Consumate = x.Consumate,
                Ramase = x.Ramase
            })
            .ToList();

        return Ok(new HR.Gateway.Api.Contracts.Concedii.Common.ConcediuCalculateDaysResponse
        {
            NumarZile = result.NumarZile,
            PeAnDetaliat = detaliat
        });
    }

    // POST api/cereri-concedii/fara-plata
    [HttpPost]
    [ProducesResponseType(typeof(CreareCerereConcediuFaraPlataResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreareCerereConcediuFaraPlataRequest body,
        CancellationToken ct)
    {
        if (body is null) return BadRequest();

        // Folose?te emailul din body, iar daca lipse?te, ia-l din user-ul logat.
        var email = string.IsNullOrWhiteSpace(body.Email)
            ? await GetCurrentEmailAsync()
            : body.Email.Trim();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Nu s-a putut determina adresa de email.");

        if (body.DataSfarsit < body.DataInceput)
            return BadRequest("DataSfarsit trebuie sa fie >= DataInceput.");

        var req = new ApplicationCerereFaraPlata.CerereConcediuFaraPlataCreateRequest
        {
            Email              = email,
            DataInceput        = body.DataInceput,
            DataSfarsit        = body.DataSfarsit,
            EmailInlocuitor    = body.EmailInlocuitor,
            NumarZileCalculate = body.NumarZileCalculate,
            Motiv      = body.Motiv
        };

        var id = await writer.CreeazaCerereAsync(req, ct);

        var resp = new CreareCerereConcediuFaraPlataResponse
        {
            Success = true,
            Message = "Cererea de concediu fara plata a fost creata cu succes.",
            CerereConcediuFaraPlataId = id
        };

        return StatusCode(StatusCodes.Status201Created, resp);
    }

    // GET api/cereri-concedii/fara-plata/{id}/document
    [HttpGet("{id:int}/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAsync(int id, CancellationToken ct)
    {
        var doc = await reader.GetDocumentAsync(id, ct);
        if (doc is null)
            return NotFound("Documentul pentru aceasta cerere nu a fost gasit.");

        // ASP.NET Core va avea grija sa �nchida stream-ul
        return File(doc.Content, doc.ContentType, doc.FileName);
    }

    // PUT api/cereri-concedii/fara-plata/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] ActualizeazaCerereConcediuFaraPlataRequest body,
        CancellationToken ct)
    {
        if (body is null) return BadRequest();

        if (body.DataSfarsit < body.DataInceput)
            return BadRequest("DataSfarsit trebuie sa fie >= DataInceput.");

        // Email: daca nu vine �n body, �l deducem din user (VAF-ul tau cere Email azi)
        var email = string.IsNullOrWhiteSpace(body.Email)
            ? await GetCurrentEmailAsync()
            : body.Email.Trim();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Nu s-a putut determina adresa de email.");

        var req = new ApplicationCerereFaraPlata.CerereConcediuFaraPlataUpdateRequest
        {
            CerereId            = id,
            Email               = email,
            EmailInlocuitor     = body.EmailInlocuitor,
            DataInceput         = body.DataInceput,
            DataSfarsit         = body.DataSfarsit,
            NumarZileCalculate  = body.NumarZileCalculate,
            Motiv       = body.Motiv
        };

        await writer.ActualizeazaCerereAsync(req, ct);
        return NoContent();
    }

    // GET api/cereri-concedii/fara-plata/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CerereConcediuFaraPlataGetByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CerereConcediuFaraPlataGetByIdResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var result = await reader.GetByIdAsync(id, ct);

        if (result is null)
            return NotFound("Cererea nu a fost gasita.");

        var response = new CerereConcediuFaraPlataGetByIdResponse
        {
            Id = result.Id,
            DataCerere = result.DataCreare,
            DataInceput = result.DataInceput,
            DataSfarsit = result.DataSfarsit,
            NumarZile = result.NumarZile,
            Stare = result.Stare,
            Inlocuitor = result.Inlocuitor,
            NumarInregistrare = result.NumarInregistrare ?? string.Empty,
            DataInregistrare = result.DataInregistrare,
            Motiv = result.Motiv
        };

        return Ok(response);
    }

    // POST api/cereri-concedii/fara-plata/{id}/inregistrare
    [HttpPost("{id:int}/inregistrare")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync(int id, CancellationToken ct)
    {
        await writer.InregistreazaCerereAsync(id, ct);
        return NoContent();
    }

    // POST api/cereri-concedii/fara-plata/{id}/semnare-electronica
    [HttpPost("{id:int}/semnare-electronica")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendToEsignAsync(int id, CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest("Id invalid.");

        await writer.TrimiteInSemnareElectronicaAsync(id, ct);
        return NoContent();
    }

    // POST api/cereri-concedii/fara-plata/{id}/incarca-semnat
    [HttpPost("{id:int}/incarca-semnat")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UploadSignedAsync(int id, IFormFile file, CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest("Id invalid.");

        if (file == null || file.Length == 0)
            return BadRequest("Fisier lipsa.");

        if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)
            && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Trebuie incarcat un fisier PDF.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            await writer.IncarcaDocumentSemnatAsync(id, file.FileName, stream, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    // POST api/cereri-concedii/fara-plata/{id}/la-aprobare
    [HttpPost("{id:int}/la-aprobare")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendForApprovalAsync(int id, CancellationToken ct)
    {
        if (id <= 0) return BadRequest("Id invalid.");

        await writer.TrimiteInAprobareAvizareAsync(id, ct);
        return Ok(new { Success = true, Message = "Cererea a fost trimisa in aprobare/avizare." });
    }
    

    [HttpGet("inlocuitori/me")]
    [ProducesResponseType(typeof(List<CerereConcediuFaraPlataGetReplacementsItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInlocuitoriMeAsync(CancellationToken ct)
    {
        var email = await GetCurrentEmailAsync();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Nu s-a putut determina adresa de email.");

        var infos = await _inlocuitori.GetInlocuitoriPentruEmailAsync(email, ct);

        var dtos = infos
            .Select(x => new CerereConcediuFaraPlataGetReplacementsItem
            {
                NumeComplet = x.NumeComplet,
                Email       = x.Email
            })
            .ToList();

        return Ok(dtos);
    }

    // ===== helpers =====

    private async Task<string?> GetCurrentEmailAsync()
    {
        // 1) claim standard
        var claimEmail =
            User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value;

        if (!string.IsNullOrWhiteSpace(claimEmail))
            return claimEmail;

        // 2) fallback la user manager
        var u = await users.GetUserAsync(User);
        return u?.Email;
    }
}








