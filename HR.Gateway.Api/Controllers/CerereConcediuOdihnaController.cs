using HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;
using HR.Gateway.Application.Abstractions.CerereConcediu;
using HR.Gateway.Application.Abstractions.CerereConcediuOdihna;
using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppModel = HR.Gateway.Application.Models.CerereConcediuOdihna;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/cereri-concedii/odihna")]
[Authorize]
public sealed class CerereConcediuOdihnaController(
    ICerereConcediuOdihnaWriter writer,
    ICerereConcediuOdihnaReader reader,
    IInlocuitoriProvider inlocuitori,
    UserManager<AppUser> users
) : ControllerBase
{
    
    private readonly IInlocuitoriProvider _inlocuitori = inlocuitori;
    
    // POST api/cereri-concedii/odihna
    [HttpPost]
    [ProducesResponseType(typeof(CerereConcediuOdihnaCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CerereConcediuOdihnaCreateRequest body,
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

        var req = new AppModel.CerereConcediuOdihnaCreateRequest
        {
            Email              = email,
            DataInceput        = body.DataInceput,
            DataSfarsit        = body.DataSfarsit,
            EmailInlocuitor    = body.EmailInlocuitor,
            NumarZileCalculate = body.NumarZileCalculate,
            AlocareManuala     = body.AlocareManuala,
            AlocariZileConcediu = body.AlocariZileLaCerere
                .Select(x => new AppModel.CerereConcediuOdihnaAllocateDaysItem
                {
                    ConcediuPerAngajatId = x.ConcediuPerAngajatId,
                    NumarZile            = x.NumarZile
                })
                .ToArray()
        };

        var id = await writer.CreeazaCerereAsync(req, ct);

        var resp = new CerereConcediuOdihnaCreateResponse
        {
            Success = true,
            Message = "Cererea de concediu de odihna a fost creata cu succes.",
            CerereConcediuOdihnaId = id
        };

        // expune ?i un Location catre endpoint-ul de alocare
        // return CreatedAtRoute("AllocateZileLaCerere", new { id }, resp);
        return StatusCode(StatusCodes.Status201Created, resp);
    }

    // GET api/cereri-concedii/odihna/{id}/document
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
    
    // PUT api/cereri-concedii/odihna/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] CerereConcediuOdihnaUpdateRequest body,
        CancellationToken ct)
    {
        if (body is null) return BadRequest();

        if (body.DataSfarsit < body.DataInceput)
            return BadRequest("DataSfarsit trebuie sa fie >= DataInceput.");

        var req = new AppModel.CerereConcediuOdihnaUpdateRequest
        {
            CerereId = id,
            DataInceput            = body.DataInceput,
            DataSfarsit            = body.DataSfarsit,
            EmailInlocuitor        = body.EmailInlocuitor,
            Motiv                  = body.Motiv,

            NumarZileCalculate     = body.NumarZileCalculate,
            AlocareManuala         = body.AlocareManuala,
            AlocariZileConcediu    = body.AlocariZileLaCerere
                .Select(x => new AppModel.CerereConcediuOdihnaAllocateDaysItem
                {
                    ConcediuPerAngajatId = x.ConcediuPerAngajatId,
                    NumarZile            = x.NumarZile
                })
                .ToArray()
        };

        await writer.ActualizeazaCerereAsync(req, ct);
        return NoContent();
    }

    [HttpGet("inlocuitori/me")]
    [ProducesResponseType(typeof(List<CerereConcediuOdihnaGetReplacementsItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInlocuitoriMeAsync(CancellationToken ct)
    {
        var email = await GetCurrentEmailAsync();
        
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Nu s-a putut determina adresa de email.");

        var infos = await _inlocuitori.GetInlocuitoriPentruEmailAsync(email, ct);

        var dtos = infos
            .Select(x => new CerereConcediuOdihnaGetReplacementsItem
            {
                NumeComplet = x.NumeComplet,
                Email       = x.Email
            })
            .ToList();

        return Ok(dtos);
    }
    
    // POST api/cereri-concedii/odihna/{id}/inregistreaza
    [HttpPost("{id:int}/inregistreaza")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync(int id, CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest("Id invalid.");

        await writer.InregistreazaCerereAsync(id, ct);
        
        return NoContent();
    }
    
    // POST api/cereri-concedii/odihna/{id}/semnare-electronica
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
    
    // POST api/cereri-concedii/odihna/{id}/incarca-semnat
    [HttpPost("{id:int}/incarca-semnat")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UploadSignedAsync(
        int id,
        IFormFile file,
        CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest("Id invalid.");

        if (file == null || file.Length == 0)
            return BadRequest("Fi?ier lipsa.");

        // op?ional: for?am doar PDF
        if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)
            && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Trebuie �ncarcat un fi?ier PDF.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            await writer.IncarcaDocumentSemnatAsync(id, file.FileName, stream, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // documentul nu e semnat sau semnatura nu e valida
            return UnprocessableEntity(ex.Message);
        }
    }
    
    // POST api/cereri-concedii/odihna/{id}/la-aprobare
    [HttpPost("{id:int}/la-aprobare")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendForApprovalAsync(int id, CancellationToken ct)
    {
        if (id <= 0) return BadRequest("Id invalid.");

        await writer.TrimiteInAprobareAvizareAsync(id, ct);
        
        return Ok(new { Success = true, Message = "Cererea a fost trimisa �n aprobare/avizare." });
    }
    
    // GET api/cereri-concedii/odihna/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CerereConcediuOdihnaGetByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CerereConcediuOdihnaGetByIdResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        // Apel direct
        var result = await reader.GetByIdAsync(id, ct);

        if (result is null)
            return NotFound("Cererea nu a fost gasita.");

        var response = new CerereConcediuOdihnaGetByIdResponse
        {
            Id = result.Id,
            DataCerere = result.DataCreare,
            DataInceput = result.DataInceput,
            DataSfarsit = result.DataSfarsit,
            NumarZile = result.NumarZile,
            Stare = result.Stare,
            Inlocuitor = result.Inlocuitor,
            NumarInregistrare = result.NumarInregistrare ?? string.Empty,
            DataInregistrare = result.DataInregistrare
        };

        return Ok(response);
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






