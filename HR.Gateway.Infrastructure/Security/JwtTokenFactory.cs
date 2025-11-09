using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR.Gateway.Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HR.Gateway.Infrastructure.Security;

public sealed class JwtTokenFactory(IOptions<JwtOptions> opts) : ITokenFactory
{
    private readonly JwtOptions _o = opts.Value;

    public Task<string> CreateJwtAsync(Guid userId, string? email, string? userName, IEnumerable<string> roles, CancellationToken ct = default)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email ?? ""),
            new(ClaimTypes.Name, userName ?? "")
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _o.Issuer,
            audience: _o.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_o.ExpiryHours),
            signingCredentials: creds
        );
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}