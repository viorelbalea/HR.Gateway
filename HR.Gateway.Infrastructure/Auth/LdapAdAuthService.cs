using System.DirectoryServices.Protocols;
using System.Net;
using HR.Gateway.Application.Abstractions.Security;
using Microsoft.Extensions.Configuration;

namespace HR.Gateway.Infrastructure.Auth;

public class LdapAdAuthService(IConfiguration cfg) : IAdAuthService
{
    // Metoda 1: Validare clasică (User + Parolă)
    public Task<bool> ValidateAsync(string username, string password)
    {
        var (domain, server, port, useSsl) = GetConfig();

        if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(domain))
            return Task.FromResult(false);

        // normalize input: DOMAIN\user -> user
        var normalized = NormalizeAccountInput(username);
        
        // Construim UPN-ul (user@contoso.local) dacă userul a introdus doar "popescu"
        var upn = username.Contains('@') ? username : $"{normalized}@{domain}";

        using var conn = CreateConnection(server, port, useSsl);
        
        try
        {
            // Aici ne autentificăm specific cu userul și parola primite
            conn.Credential = new NetworkCredential(upn, password);
            conn.Bind(); 
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    // Metoda 2: Căutare Email pentru SSO (fără parola userului)
public Task<string?> GetEmailAsync(string username)
{
    return Task.Run(() =>
    {
        var (domain, server, port, useSsl) = GetConfig();

        if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(domain))
            return null;

        var searchBase = GetDistinguishedNameFromDomain(domain);
        if (string.IsNullOrWhiteSpace(searchBase))
            return null;

        // Normalize: DOMAIN\user / user@domain / user -> user
        var sam = NormalizeAccountInput(username);
        if (string.IsNullOrWhiteSpace(sam))
            return null;

        // Escape pentru LDAP filter
        var samEscaped = EscapeLdapFilterValue(sam);

        using var conn = CreateConnection(server, port, useSsl);

        try
        {
            // Bind cu identitatea procesului (service account / app pool user)
            conn.AuthType = AuthType.Negotiate;
            conn.Bind();

            // Căutare după sAMAccountName
            var filter = $"(&(objectClass=user)(sAMAccountName={samEscaped}))";

            // cerem mail + fallback userPrincipalName
            var request = new SearchRequest(
                searchBase,
                filter,
                SearchScope.Subtree,
                "mail",
                "userPrincipalName"
            );

            var response = (SearchResponse)conn.SendRequest(request);
            if (response.Entries.Count <= 0)
                return null;

            var entry = response.Entries[0];

            string? GetAttr(string attrName)
            {
                if (!entry.Attributes.Contains(attrName)) return null;
                if (entry.Attributes[attrName].Count <= 0) return null;
                return entry.Attributes[attrName][0]?.ToString();
            }

            var mail = GetAttr("mail");
            if (!string.IsNullOrWhiteSpace(mail))
                return mail;

            var upn = GetAttr("userPrincipalName");
            if (!string.IsNullOrWhiteSpace(upn))
                return upn;

            return null;
        }
        catch (Exception ex)
        {
            // ideal: ILogger
            Console.WriteLine($"[AD Warning] Email lookup failed for '{username}'. {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    });
}


    // --- Helper pentru citire configurare ---
    private (string? domain, string? server, int port, bool useSsl) GetConfig()
    {
        var domain = cfg["AD:Domain"];
        var server = cfg["AD:Server"] ?? domain; // Dacă lipsește serverul, încercăm pe domeniu
        var port = int.TryParse(cfg["AD:Port"], out var p) ? p : 389;
        var useSsl = bool.TryParse(cfg["AD:UseSsl"], out var s) && s;

        return (domain, server, port, useSsl);
    }

    // --- Helper conexiune ---
    private LdapConnection CreateConnection(string? server, int port, bool useSsl)
    {
        var identifier = new LdapDirectoryIdentifier(server, port);
        var conn = new LdapConnection(identifier);

        conn.SessionOptions.ProtocolVersion = 3;
        conn.SessionOptions.SecureSocketLayer = useSsl;
        // Default AuthType, îl suprascriem în metode dacă e nevoie
        conn.AuthType = AuthType.Negotiate; 
        
        return conn;
    }

    // --- Helper Transformare Domeniu -> DN ---
    private string GetDistinguishedNameFromDomain(string? domain)
    {
        if (string.IsNullOrWhiteSpace(domain)) return "";
        
        // Input: "contoso.local"
        // Split: ["contoso", "local"]
        // Prefix: ["DC=contoso", "DC=local"]
        // Join: "DC=contoso,DC=local"
        
        var parts = domain.Split('.');
        var dnParts = parts.Select(p => $"DC={p}");
        return string.Join(",", dnParts);
    }
    
    private static string NormalizeAccountInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // "DOMAIN\user" -> "user"
        if (input.Contains('\\'))
            return input.Split('\\', 2)[1];

        // "user@domain" -> "user"
        if (input.Contains('@'))
            return input.Split('@', 2)[0];

        return input.Trim();
    }

// LDAP filter escaping (RFC 4515)
    private static string EscapeLdapFilterValue(string value)
    {
        if (value is null) return string.Empty;

        return value
            .Replace(@"\",  @"\5c")
            .Replace("*",   @"\2a")
            .Replace("(",   @"\28")
            .Replace(")",   @"\29")
            .Replace("\0",  @"\00");
    }

}