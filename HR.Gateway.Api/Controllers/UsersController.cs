using HR.Gateway.Api.Contracts.Angajati;
using HR.Gateway.Api.Contracts.Concedii.Common;
using HR.Gateway.Api.Contracts.Utilizatori;
using HR.Gateway.Application.Abstractions.Angajati;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(
    UserManager<AppUser> users,
    RoleManager<AppRole> roles,
    IAngajatiReader overview
) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult GetAll()
    {
        var list = users.Users.Select(u => new { u.Id, u.Email, u.UserName }).ToList();
        return Ok(list);
    }

    [HttpPost("{id:guid}/roles/{role}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddRole(Guid id, string role)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        if (!await roles.RoleExistsAsync(role))
            await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = role });

        var res = await users.AddToRoleAsync(user, role);
        if (!res.Succeeded) return Problem(string.Join("; ", res.Errors.Select(e => e.Description)));

        return Ok(new { user.Id, role });
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return Unauthorized();
        var userRoles = await users.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, user.UserName, roles = userRoles });
    }

    [HttpGet("me/overview")]
    public async Task<ActionResult<PrezentareUtilizatorDto>> Overview(CancellationToken ct)
    {
        string? email =
            User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            var u = await users.GetUserAsync(User);
            email = u?.Email;
        }
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();


        var app = await overview.GetOverviewAsync(email, ct);
        if (app is null) return NotFound();

        var peAniDetaliat = app.PeAniDetaliat?
            .Select(p => new ConcediuPeAnDetaliatDto
            {
                An        = p.An,
                AnId      = p.AnId,
                Alocate   = p.Alocate,
                Consumate = p.Consumate,
                Ramase    = p.Ramase
            })
            .ToList();

        var totalDisponibile = peAniDetaliat?.Sum(x => x.Ramase) ?? app.TotalZileDisponibile;
        
        var profil = new ProfilAngajatDto
        {
            Nume               = app.Profil.Nume ?? "",
            Prenume            = app.Profil.Prenume ?? "",
            NumeComplet        = app.Profil.NumeComplet,
            Email              = app.Profil.Email,
            Departament        = app.Profil.Departament ?? "",
            Functie            = app.Profil.Functie ?? "",
            ZileConcediuRamase = totalDisponibile 
        };

        var concedii = new SituatieConcediiAngajatDto
        {
            TotalDisponibile = totalDisponibile,
            PeAniDetaliat    = peAniDetaliat
        };

        var dto = new PrezentareUtilizatorDto
        {
            Profil = profil,
            SituatieConcedii = concedii
        };

        return Ok(dto);
    }
}
