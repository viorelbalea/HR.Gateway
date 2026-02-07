using System.Net.Http.Json;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Overview;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Cereri;
using Microsoft.Extensions.Logging;

namespace HR.Gateway.Infrastructure.Angajat.Client;

internal sealed class VemAngajatClient(HttpClient http, ILogger<VemAngajatClient> logger) : IVemAngajatClient
{
    public async Task<VemRaspunsOverview?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        var url = "vault/extensionmethod/GetAngajatDataExtensionMethod";
        var fullUrl = new Uri(http.BaseAddress!, url);

        logger.LogInformation("Calling M-Files: POST {FullUrl} with email={Email}", fullUrl, email);

        using var resp = await http.PostAsJsonAsync(url, body, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var content = await resp.Content.ReadAsStringAsync(ct);
            logger.LogError("M-Files returned {StatusCode} for {Url}: {Content}",
                resp.StatusCode, fullUrl, content);
        }

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<VemRaspunsOverview>(cancellationToken: ct);
    }

    public async Task<VemRaspunsCereriConcediuOdihna?> GetHolidayRequestsByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        using var resp = await http.PostAsJsonAsync("vault/extensionmethod/GetAngajatHolidayRequestsDataExtensionMethod", body, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<VemRaspunsCereriConcediuOdihna>(cancellationToken: ct);
    }

    public async Task<VemRaspunsCereriConcediuOdihna?> GetUnpaidLeaveRequestsByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        using var resp = await http.PostAsJsonAsync("vault/extensionmethod/GetAngajatConcediiFaraPlataDataExtensionMethod", body, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<VemRaspunsCereriConcediuOdihna>(cancellationToken: ct);
    }

    public async Task<VemRaspunsCereriConcediuOdihna?> GetSpecialEventLeaveRequestsByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        using var resp = await http.PostAsJsonAsync("vault/extensionmethod/GetAngajatConcediiEvenimentExtensionMethod", body, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<VemRaspunsCereriConcediuOdihna>(cancellationToken: ct);
    }
}
