using HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;
using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Application.Models.CereriConcediu;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/cereri-concedii/odihna")]
[Authorize]
public sealed class CereriConcediiOdihnaController(
    ICereriConcediiWriter writer,
    UserManager<AppUser> users
) : ControllerBase
{
    // POST api/cereri-concedii/odihna
    [HttpPost]
    [ProducesResponseType(typeof(CreareConcediuOdihnaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreareConcediuOdihnaRequest body,
        CancellationToken ct)
    {
        if (body is null) return BadRequest();

        // Folosește emailul din body, iar dacă lipsește, ia-l din user-ul logat.
        var email = string.IsNullOrWhiteSpace(body.Email)
            ? await GetCurrentEmailAsync()
            : body.Email.Trim();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Nu s-a putut determina adresa de email.");

        if (body.DataSfarsit < body.DataInceput)
            return BadRequest("DataSfarsit trebuie să fie >= DataInceput.");

        var req = new CreareCerereConcediuOdihnaReq
        {
            Email              = email,
            DataInceput        = body.DataInceput,
            DataSfarsit        = body.DataSfarsit,
            EmailInlocuitor    = body.EmailInlocuitor,
            NumarZileCalculate = body.NumarZileCalculate,
            AlocareManuala     = body.AlocareManuala
        };

        var id = await writer.CreeazaCerereAsync(req, ct);

        var resp = new CreareConcediuOdihnaResponse
        {
            Success = true,
            Message = "Cererea de concediu de odihnă a fost creată cu succes.",
            CerereConcediuOdihnaId = id
        };

        // expune și un Location către endpoint-ul de alocare
        return CreatedAtRoute("AllocateZileLaCerere", new { id }, resp);
    }

    // POST api/cereri-concedii/odihna/{id}/alocari
    [HttpPost("{id:int}/alocari", Name = "AllocateZileLaCerere")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AllocateAsync(
        int id,
        [FromBody] AlocareZileLaCerereConcediuRequest body,
        CancellationToken ct)
    {
        if (body is null || body.AlocariZileLaCerere is null || body.AlocariZileLaCerere.Count == 0)
            return BadRequest("Lista 'AlocariZileLaCerere' este goală.");

        // dacă body are alt id, îl ignorăm și folosim cel din rută
        var req = new AlocareZileLaCerereConcediuOdihnaReq
        {
            CerereConcediuId = id,
            AlocariZileConcediu = body.AlocariZileLaCerere
                .Select(x => new AlocareZileLaCerereConcediuOdihnaReq.Item
                {
                    ConcediuPerAngajatId = x.ConcediuPerAngajatId,
                    NumarZile            = x.NumarZile
                })
                .ToList()
        };

        await writer.AlocaZileAsync(req, ct);

        return Ok(new
        {
            Success = true,
            Message = "Zilele de concediu au fost alocate cu succes la cererea de concediu de odihnă."
        });
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
