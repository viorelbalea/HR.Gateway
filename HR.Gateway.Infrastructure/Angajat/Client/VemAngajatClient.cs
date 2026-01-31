using System.Net.Http.Json;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Overview;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Cereri;

namespace HR.Gateway.Infrastructure.Angajat.Client;

internal sealed class VemAngajatClient(HttpClient http) : IVemAngajatClient
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

    public async Task<VemRaspunsCereriConcediuOdihna?> GetUnpaidLeaveRequestsByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        using var resp = await http.PostAsJsonAsync("vault/extensionmethod/GetAngajatUnpaidLeaveRequestsDataExtensionMethod", body, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<VemRaspunsCereriConcediuOdihna>(cancellationToken: ct);
    }

    public async Task<VemRaspunsCereriConcediuOdihna?> GetSpecialEventLeaveRequestsByEmailAsync(string email, CancellationToken ct)
    {
        var body = new { Email = email };
        using var resp = await http.PostAsJsonAsync("vault/extensionmethod/GetAngajatSpecialEventLeaveRequestsDataExtensionMethod", body, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<VemRaspunsCereriConcediuOdihna>(cancellationToken: ct);
    }
}
