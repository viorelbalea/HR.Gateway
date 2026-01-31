using System.Net.Http.Json;
using HR.Gateway.Infrastructure.Concediu.Client.Dtos;

namespace HR.Gateway.Infrastructure.Concediu.Client;

internal sealed class VemConcediuService(HttpClient http) : IVemConcediuService
{
    private const string Path = "/REST/vault/extensionmethod/CalculateZileConcediuForWPFDesktopExtensionMethod";

    public async Task<VemCalculeazaZileResponse> CalculeazaZileAsync(
        string email, DateTime start, DateTime end, CancellationToken ct)
    {
        var body = new VemCalculeazaZileRequest
        {
            Email       = email.Trim().ToLowerInvariant(),
            DataInceput = DateTime.SpecifyKind(start, DateTimeKind.Unspecified),
            DataSfarsit = DateTime.SpecifyKind(end,   DateTimeKind.Unspecified)
        };

        using var resp = await http.PostAsJsonAsync(Path, body, ct);
        resp.EnsureSuccessStatusCode();

        var payload = await resp.Content.ReadFromJsonAsync<VemCalculeazaZileResponse>(cancellationToken: ct)
                      ?? throw new InvalidOperationException("Empty VEM response.");

        if (!payload.FoundSuccess)
            throw new InvalidOperationException("VEM calculează: FoundSuccess = false.");

        return payload;
    }
}

