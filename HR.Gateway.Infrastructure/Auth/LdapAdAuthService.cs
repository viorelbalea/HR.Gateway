using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Configuration;

using HR.Gateway.Application.Abstractions.Security;

namespace HR.Gateway.Infrastructure.Auth;

public class LdapAdAuthService(IConfiguration cfg) : IAdAuthService
{
    public Task<bool> ValidateAsync(string username, string password)
    {
        var domain = cfg["AD:Domain"];
        var server = cfg["AD:Server"] ?? domain;
        var port   = int.TryParse(cfg["AD:Port"], out var p) ? p : 389;
        var useSsl = bool.TryParse(cfg["AD:UseSsl"], out var s) && s;

        if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(domain))
            return Task.FromResult(false);

        // prefer UPN
        var upn = username.Contains('@') ? username : $"{username}@{domain}";

        using var conn = new LdapConnection(new LdapDirectoryIdentifier(server, port));
        conn.SessionOptions.ProtocolVersion = 3;
        conn.SessionOptions.SecureSocketLayer = useSsl;
        conn.AuthType = useSsl ? AuthType.Basic : AuthType.Negotiate;
        conn.Credential = new NetworkCredential(upn, password);

        try
        {
            conn.Bind(); // dacă nu aruncă, credentialele sunt OK
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}