using HR.Gateway.Api.Contracts.Concedii.ConcediuLaEveniment;
using HR.Gateway.Application.Abstractions.CerereConcediu;
using HR.Gateway.Application.Abstractions.CerereConcediuLaEveniment;
using HR.Gateway.Application.Abstractions.Concedii;
using ApplicationCerereEveniment = HR.Gateway.Application.Models.CerereConcediuLaEveniment;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/cereri-concedii/eveniment")]
[Authorize]
public sealed class CerereConcediuLaEvenimentController(
    ICerereConcediuLaEvenimentWriter writer,
    ICerereConcediuLaEvenimentReader reader,
    IConcediuCalculator calculator,
    IInlocuitoriProvider inlocuitori,
    UserManager<AppUser> users
) : ControllerBase
{
    private readonly IInlocuitoriProvider _inlocuitori = inlocuitori;

    [HttpPost("calculeaza-zile")]
    public async Task<ActionResult<HR.Gateway.Api.Contracts.Concedii.Common.ConcediuCalculateDaysResponse>> CalculeazaZileAsync(
        [FromBody] HR.Gateway.Api.Contracts.Concedii.Common.ConcediuCalculateDaysRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email)) return BadRequest("Email required.");
        
        var start = DateOnly.FromDateTime(req.DataInceput);
        var end = DateOnly.FromDateTime(req.DataSfarsit);

        var result = await calculator.CalculeazaZileAsync(req.Email, start, end, ct);

        return Ok(new HR.Gateway.Api.Contracts.Concedii.Common.ConcediuCalculateDaysResponse
        {
            NumarZile = result.NumarZile,
            PeAnDetaliat = result.PeAnDetaliat.Select(x => new HR.Gateway.Api.Contracts.Concedii.Common.ConcediuYearDetailItem
            {
                An = x.An,
                AnId = x.AnId,
                Alocate = x.Alocate,
                Consumate = x.Consumate,
                Ramase = x.Ramase
            }).ToList()
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(CerereConcediuLaEvenimentCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CerereConcediuLaEvenimentCreateRequest body, CancellationToken ct)
    {
        if (body is null) return BadRequest();
        var email = string.IsNullOrWhiteSpace(body.Email) ? await GetCurrentEmailAsync() : body.Email.Trim();
        if (string.IsNullOrWhiteSpace(email)) return BadRequest("Nu s-a putut determina emailul.");

        var req = new ApplicationCerereEveniment.CerereConcediuLaEvenimentCreateRequest
        {
            Email = email,
            DataInceput = body.DataInceput,
            DataSfarsit = body.DataSfarsit,
            EmailInlocuitor = body.EmailInlocuitor,
            NumarZileCalculate = body.NumarZileCalculate,
            Motiv = body.Motiv
        };

        var id = await writer.CreeazaCerereAsync(req, ct);
        return StatusCode(StatusCodes.Status201Created, new CerereConcediuLaEvenimentCreateResponse
        {
            Success = true,
            Message = "Cererea de concediu de eveniment a fost creata.",
            CerereConcediuEvenimentId = id
        });
    }

    [HttpGet("{id:int}/document")]
    public async Task<IActionResult> DownloadAsync(int id, CancellationToken ct)
    {
        var doc = await reader.GetDocumentAsync(id, ct);
        if (doc is null) return NotFound();
        return File(doc.Content, doc.ContentType, doc.FileName);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CerereConcediuLaEvenimentUpdateRequest body, CancellationToken ct)
    {
        if (body is null) return BadRequest();
        var email = string.IsNullOrWhiteSpace(body.Email) ? await GetCurrentEmailAsync() : body.Email.Trim();

        var req = new ApplicationCerereEveniment.CerereConcediuLaEvenimentUpdateRequest
        {
            CerereId = id,
            Email = email!,
            EmailInlocuitor = body.EmailInlocuitor,
            DataInceput = body.DataInceput,
            DataSfarsit = body.DataSfarsit,
            NumarZileCalculate = body.NumarZileCalculate,
            Motiv = body.Motiv
        };

        await writer.ActualizeazaCerereAsync(req, ct);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CerereConcediuLaEvenimentGetByIdResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var result = await reader.GetByIdAsync(id, ct);
        if (result is null) return NotFound();

        return Ok(new CerereConcediuLaEvenimentGetByIdResponse
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
        });
    }

    [HttpPost("{id:int}/inregistrare")]
    public async Task<IActionResult> RegisterAsync(int id, CancellationToken ct)
    {
        await writer.InregistreazaCerereAsync(id, ct);
        return NoContent();
    }

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
    [ProducesResponseType(typeof(List<CerereConcediuLaEvenimentGetReplacementsItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInlocuitoriMeAsync(CancellationToken ct)
    {
        var email = await GetCurrentEmailAsync();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Nu s-a putut determina adresa de email.");

        var infos = await _inlocuitori.GetInlocuitoriPentruEmailAsync(email, ct);

        var dtos = infos
            .Select(x => new CerereConcediuLaEvenimentGetReplacementsItem
            {
                NumeComplet = x.NumeComplet,
                Email       = x.Email
            })
            .ToList();

        return Ok(dtos);
    }

    private async Task<string?> GetCurrentEmailAsync()
    {
        var claimEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
        if (!string.IsNullOrWhiteSpace(claimEmail)) return claimEmail;
        var u = await users.GetUserAsync(User);
        return u?.Email;
    }
}







