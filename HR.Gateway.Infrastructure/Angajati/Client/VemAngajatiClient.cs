using System.Net.Http.Json;
using HR.Gateway.Infrastructure.Angajati.Client.Dtos.Overview;
using HR.Gateway.Infrastructure.Angajati.Client.Dtos.Cereri;

namespace HR.Gateway.Infrastructure.Angajati.Client;

internal sealed class VemAngajatiClient(HttpClient http) : IVemAngajatiClient
{
    public async Task<VemRaspunsOverview?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        using var resp = await http.PostAsJsonAsync("vault/extensionmethod/GetAngajatDataExtensionMethod", body, ct);
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
}