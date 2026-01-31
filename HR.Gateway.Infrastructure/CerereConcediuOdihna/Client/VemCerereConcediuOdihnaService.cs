// HR.Gateway.Infrastructure.CerereConcediuOdihna.Client/VemCerereConcediuOdihnaService.cs
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client;

internal sealed class VemCerereConcediuOdihnaService(HttpClient http) : IVemCerereConcediuOdihnaService
{
    private static readonly JsonSerializerOptions json =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<CerereConcediuOdihnaCreateResponse> CreateAsync(CerereConcediuOdihnaCreateRequest req, CancellationToken ct)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/CreateHolidayRequestDocumentExtensionMethod",
            req, json, ct);

        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaCreateResponse>(json, ct)
                  ?? new CerereConcediuOdihnaCreateResponse { /* defaults */ };

        return dto;
    }

    public async Task<CerereConcediuOdihnaCreateResponse> UpdateAsync(CerereConcediuOdihnaUpdateRequest req, CancellationToken ct)
    {
        // numele VEM-ului de update – îl alegi tu în VAF
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/UpdateHolidayRequestDocumentExtensionMethod",
            req, json, ct);

        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaCreateResponse>(json, ct)
                  ?? new CerereConcediuOdihnaCreateResponse { };
        return dto;
    }
    
    public async Task<CerereConcediuOdihnaGetReplacementsResponse> GetReplacementsAsync(
        CerereConcediuOdihnaGetReplacementsRequest req,
        CancellationToken ct)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/GetPotentialReplacementsExtensionMethod",
            req, json, ct);

        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaGetReplacementsResponse>(json, ct)
                  ?? new CerereConcediuOdihnaGetReplacementsResponse() { Succes = false, Mesaj = "Răspuns gol de la VEM." };

        return dto;
    }
    
    public async Task<CerereConcediuOdihnaRegisterResponse> RegisterAsync(
        CerereConcediuOdihnaRegisterRequest req,
        CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/RegisterHolidayRequestDocumentExtensionMethod",
            req,
            json,
            ct);

        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaRegisterResponse>(json, ct)
                  ?? new CerereConcediuOdihnaRegisterResponse() { Succes = false, Mesaj = "Răspuns gol de la VEM." };

        return dto;
    }
    
    public async Task<CerereConcediuOdihnaSendToEsignResponse> SendToEsignAsync(
        CerereConcediuOdihnaSendToEsignRequest req,
        CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/SendHolidayRequestDocumentToEsignExtensionMethod",
            req,
            json,
            ct);

        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaSendToEsignResponse>(json, ct)
                  ?? new CerereConcediuOdihnaSendToEsignResponse { Succes = false, Mesaj = "Răspuns gol de la VEM." };

        return dto;
    }
    
    public async Task<CerereConcediuOdihnaUploadSignedResponse> UploadSignedAsync(
        CerereConcediuOdihnaUploadSignedRequest req,
        CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/UploadSignedHolidayRequestDocumentExtensionMethod",
            req,
            json,
            ct);

        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaUploadSignedResponse>(json, ct)
                  ?? new CerereConcediuOdihnaUploadSignedResponse() { Succes = false, Mesaj = "Răspuns gol de la VEM." };

        return dto;
    }
    
    public async Task<CerereConcediuOdihnaSendForApprovalResponse> SendForApprovalAsync(
        CerereConcediuOdihnaSendForApprovalRequest req,
        CancellationToken ct = default)
    {

        var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/SendHolidayRequestForApprovalExtensionMethod",
            req,
            json,
            ct);

        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaSendForApprovalResponse>(json, ct)
                  ?? new CerereConcediuOdihnaSendForApprovalResponse { };

        return dto;
    }

    public async Task<CerereConcediuOdihnaGetByIdResponse?> GetByIdAsync(
        CerereConcediuOdihnaGetByIdRequest req, 
        CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/GetHolidayRequestDetailsExtensionMethod",
            req,
            json,
            ct);

        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<CerereConcediuOdihnaGetByIdResponse>(json, ct);
        return dto;
    }
}




