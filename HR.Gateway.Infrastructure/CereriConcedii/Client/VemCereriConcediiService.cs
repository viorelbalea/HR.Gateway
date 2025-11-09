// HR.Gateway.Infrastructure.CereriConcedii.Client/VemCereriConcediiService.cs
using System.Net.Http.Json;
using System.Text.Json;
using HR.Gateway.Infrastructure.CereriConcedii.Client.Dtos;

namespace HR.Gateway.Infrastructure.CereriConcedii.Client;

internal sealed class VemCereriConcediiService(HttpClient http) : IVemCereriConcediiService
{
    private static readonly JsonSerializerOptions json =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<CreateResp> CreateAsync(
        CreateReq req, CancellationToken ct)
    {
        // /REST/vault/extensionmethod/CreateHolidayRequestDocumentExtensionMethod
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/CreateHolidayRequestDocumentExtensionMethod",
            req, json, ct);

        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<CreateResp>(json, ct)
                  ?? new CreateResp { /* defaults */ };
        return dto;
    }

    public async Task<AllocResp> AllocateAsync(
        AllocReq req, CancellationToken ct)
    {
        // /REST/vault/extensionmethod/AlocareZileConcediuLaCerereExtensionMethod
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/AlocareZileConcediuLaCerereExtensionMethod",
            req, json, ct);

        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<AllocResp>(json, ct)
                  ?? new AllocResp { /* defaults */ };
        return dto;
    }
}