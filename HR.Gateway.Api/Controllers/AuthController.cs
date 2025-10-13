using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<AppUser> users,
    RoleManager<AppRole> roles,
    IConfiguration cfg
) : ControllerBase
{
    // POST /api/auth/register 
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email,
            EmailConfirmed = true
        };

        var create = await users.CreateAsync(user, dto.Password);
        if (!create.Succeeded) return Problem(string.Join("; ", create.Errors.Select(e => e.Description)));

        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            if (!await roles.RoleExistsAsync(dto.Role))
                await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = dto.Role });

            await users.AddToRoleAsync(user, dto.Role);
        }

        return Ok(new { user.Id, user.Email, dto.Role });
    }

    // POST /api/auth/login  (JWT local, cu user/parolă din Identity)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await users.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized();
        if (!await users.CheckPasswordAsync(user, dto.Password)) return Unauthorized();

        return Ok(new { token = CreateJwt(user) });
    }

    [HttpPost("login-ad")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAd(
        [FromBody] LoginDto dto,
        [FromServices] HR.Gateway.Application.Abstractions.Security.IAdAuthService adAuth,
        [FromServices] UserManager<AppUser> users,
        [FromServices] RoleManager<AppRole> roles,
        [FromServices] IConfiguration cfg)
    {
        var ok = await adAuth.ValidateAsync(dto.Email, dto.Password);
        if (!ok) return Unauthorized();

        // mapăm userul AD în user local (creăm dacă nu există)
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
            if (!create.Succeeded) return Problem(string.Join("; ", create.Errors.Select(e => e.Description)));
            // opțional: rol implicit
            const string defaultRole = "Employee";
            if (!await roles.RoleExistsAsync(defaultRole))
                await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = defaultRole });
            await users.AddToRoleAsync(user, defaultRole);
        }

        // generează JWT identic cu login-ul local
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.UserName ?? "")
        };
        var userRoles = await users.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: cfg["JWT:Issuer"],
            audience: cfg["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me([FromServices] UserManager<AppUser> users)
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return Unauthorized();
        var roles = await users.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, user.UserName, roles });
    }

    // — helpers —
    private string CreateJwt(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.UserName ?? "")
        };

        var userRoles = users.GetRolesAsync(user).GetAwaiter().GetResult();
        claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: cfg["JWT:Issuer"],
            audience: cfg["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // DTOs
    public record RegisterDto(string Email, string Password, string? Role);
    public record LoginDto(string Email, string Password);
}
