using HR.Gateway.Api.Contracts.Utilizatori.Auth;
using HR.Gateway.Application.Abstractions.Security;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<AppUser> users,
    RoleManager<AppRole> roles,
    ITokenFactory tokens,    
    HR.Gateway.Application.Abstractions.Security.IAdAuthService? adAuth = null 
) : ControllerBase
{
    
    
    // POST /api/auth/register 
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UtilizatorRegisterResponse>> Register([FromBody] UtilizatorRegisterRequest dto)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email,
            EmailConfirmed = true
        };

        var create = await users.CreateAsync(user, dto.Password);
        if (!create.Succeeded)
            return Problem(string.Join("; ", create.Errors.Select(e => e.Description)));

        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            if (!await roles.RoleExistsAsync(dto.Role))
                await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = dto.Role });

            await users.AddToRoleAsync(user, dto.Role);
        }

        return Ok(new UtilizatorRegisterResponse { Id = user.Id, Email = user.Email!, Role = dto.Role });
    }

    // POST /api/auth/login  (JWT local)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<UtilizatorLoginResponse>> Login([FromBody] UtilizatorLoginRequest dto)
    {
        var user = await users.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized();
        if (!await users.CheckPasswordAsync(user, dto.Password)) return Unauthorized();

        var rolesList = await users.GetRolesAsync(user);
        var token = await tokens.CreateJwtAsync(user.Id, user.Email, user.UserName, rolesList);

        return Ok(new UtilizatorLoginResponse { Token = token });
    }

    // POST /api/auth/login-ad  (LDAP/AD + mapare user local + JWT)
    [HttpPost("login-ad")]
    [AllowAnonymous]
    public async Task<ActionResult<UtilizatorLoginResponse>> LoginAd([FromBody] UtilizatorLoginRequest dto)
    {
        if (adAuth is null) return Problem("AD auth service not configured.");
        var ok = await adAuth.ValidateAsync(dto.Email, dto.Password);
        if (!ok) return Unauthorized();

        var user = await users.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                EmailConfirmed = true
            };
            var create = await users.CreateAsync(user);
            if (!create.Succeeded)
                return Problem(string.Join("; ", create.Errors.Select(e => e.Description)));

            const string defaultRole = "Employee";
            if (!await roles.RoleExistsAsync(defaultRole))
                await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = defaultRole });
            await users.AddToRoleAsync(user, defaultRole);
        }

        var rolesList = await users.GetRolesAsync(user);
        var token = await tokens.CreateJwtAsync(user.Id, user.Email, user.UserName, rolesList);

        return Ok(new UtilizatorLoginResponse { Token = token });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return Unauthorized();
        var userRoles = await users.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, user.UserName, roles = userRoles });
    }
    
    [HttpPost("sso")]
    [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
    public async Task<ActionResult<UtilizatorLoginResponse>> Sso()
    {
        if (adAuth is null) return Problem("AD auth service not configured.");

        // 1) Identitatea Windows (ex: DOMAIN\\popescu)
        var winName = User?.Identity?.Name;
        if (string.IsNullOrWhiteSpace(winName)) return Unauthorized();

        // 2) Extragem sAMAccountName (partea după \)
        var sam = winName.Contains('\\') ? winName.Split('\\')[1] : winName;

        // 3) Căutăm email în AD (mail, fallback upn)
        var email = await adAuth.GetEmailAsync(sam);
        if (string.IsNullOrWhiteSpace(email))
            return Problem($"AD user '{sam}' has no email (mail/userPrincipalName).");

        // 4) Mapare/creare user local (Identity)
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var create = await users.CreateAsync(user);
            if (!create.Succeeded)
                return Problem(string.Join("; ", create.Errors.Select(e => e.Description)));

            const string defaultRole = "Employee";
            if (!await roles.RoleExistsAsync(defaultRole))
                await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = defaultRole });
            await users.AddToRoleAsync(user, defaultRole);
        }

        // 5) JWT
        var rolesList = await users.GetRolesAsync(user);
        var token = await tokens.CreateJwtAsync(user.Id, user.Email, user.UserName, rolesList);

        return Ok(new UtilizatorLoginResponse { Token = token });
    }

}
