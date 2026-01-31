using System.Net.Http.Json;
using System.Text.Json;
using HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client;

internal sealed class VemCerereConcediuLaEvenimentService(HttpClient http) : IVemCerereConcediuLaEvenimentService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<CerereConcediuLaEvenimentCreateResponse> CreateAsync(CerereConcediuLaEvenimentCreateRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/CreateSpecialEventLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentCreateResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentCreateResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentCreateResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

    public async Task<CerereConcediuLaEvenimentUpdateResponse> UpdateAsync(CerereConcediuLaEvenimentUpdateRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/UpdateSpecialEventLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentUpdateResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentUpdateResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentUpdateResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

    public async Task<CerereConcediuLaEvenimentGetByIdResponse?> GetByIdAsync(CerereConcediuLaEvenimentGetByIdRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/GetSpecialEventLeaveRequestDetailsExtensionMethod",
            req, JsonOptions, ct);

        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentGetByIdResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentGetByIdResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentGetByIdResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
    
    public async Task<CerereConcediuLaEvenimentRegisterResponse> RegisterAsync(CerereConcediuLaEvenimentRegisterRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/RegisterSpecialEventLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentRegisterResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentRegisterResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentRegisterResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
    
    public async Task<CerereConcediuLaEvenimentSendToEsignResponse> SendToEsignAsync(CerereConcediuLaEvenimentSendToEsignRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/SendSpecialEventLeaveRequestToEsignExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentSendToEsignResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentSendToEsignResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentSendToEsignResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

    public async Task<CerereConcediuLaEvenimentUploadSignedResponse> UploadSignedAsync(CerereConcediuLaEvenimentUploadSignedRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/UploadSignedSpecialEventLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentUploadSignedResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentUploadSignedResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentUploadSignedResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
    
    public async Task<CerereConcediuLaEvenimentSendForApprovalResponse> SendForApprovalAsync(CerereConcediuLaEvenimentSendForApprovalRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/SendSpecialEventLeaveRequestForApprovalExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuLaEvenimentSendForApprovalResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuLaEvenimentSendForApprovalResponse>(content, JsonOptions)
               ?? new CerereConcediuLaEvenimentSendForApprovalResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
}








