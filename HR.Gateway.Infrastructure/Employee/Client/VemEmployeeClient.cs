// HR.Gateway.Infrastructure/Employee/Client/VemEmployeeClient.cs
using System.Net.Http.Json;

namespace HR.Gateway.Infrastructure.Employee.Client;

internal sealed class VemEmployeeClient : IVemEmployeeClient
{
    private readonly HttpClient _http;
    public VemEmployeeClient(HttpClient http) => _http = http;

    public async Task<Dtos.VemAngajatResponse?> GetByEmailAsync(string email, CancellationToken ct)
    {
        // baza e deja http://localhost/REST/ (din DI)
        var url = "vault/extensionmethod/GetAngajatDataExtensionMethod";
        var payload = new { Email = email };

        using var res = await _http.PostAsJsonAsync(url, payload, ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"VEM {(int)res.StatusCode}: {res.ReasonPhrase}. {body}");
        }

        return await res.Content.ReadFromJsonAsync<Dtos.VemAngajatResponse>(cancellationToken: ct);
    }
}