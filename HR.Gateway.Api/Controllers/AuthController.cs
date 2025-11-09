using HR.Gateway.Api.Contracts.Utilizatori.Auth;
using HR.Gateway.Application.Abstractions.Security;
using HR.Gateway.Infrastructure.Entities;
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
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest dto)
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

        return Ok(new RegisterResponse { Id = user.Id, Email = user.Email!, Role = dto.Role });
    }

    // POST /api/auth/login  (JWT local)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest dto)
    {
        var user = await users.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized();
        if (!await users.CheckPasswordAsync(user, dto.Password)) return Unauthorized();

        var rolesList = await users.GetRolesAsync(user);
        var token = await tokens.CreateJwtAsync(user.Id, user.Email, user.UserName, rolesList);

        return Ok(new LoginResponse { Token = token });
    }

    // POST /api/auth/login-ad  (LDAP/AD + mapare user local + JWT)
    [HttpPost("login-ad")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> LoginAd([FromBody] LoginRequest dto)
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

        return Ok(new LoginResponse { Token = token });
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
}
