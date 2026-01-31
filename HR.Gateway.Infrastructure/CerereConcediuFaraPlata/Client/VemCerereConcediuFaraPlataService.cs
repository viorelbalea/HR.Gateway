using System.Net.Http.Json;
using System.Text.Json;
using HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client;

internal sealed class VemCerereConcediuFaraPlataService(HttpClient http) : IVemCerereConcediuFaraPlataService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<CerereConcediuFaraPlataCreateResponse> CreateAsync(CerereConcediuFaraPlataCreateRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/CreateUnpaidLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataCreateResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataCreateResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataCreateResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

    public async Task<CerereConcediuFaraPlataUpdateResponse> UpdateAsync(CerereConcediuFaraPlataUpdateRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/UpdateUnpaidLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataUpdateResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataUpdateResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataUpdateResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

    public async Task<CerereConcediuFaraPlataGetByIdResponse?> GetByIdAsync(CerereConcediuFaraPlataGetByIdRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/GetUnpaidLeaveRequestDetailsExtensionMethod",
            req, JsonOptions, ct);

        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataGetByIdResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataGetByIdResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataGetByIdResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
    
    public async Task<CerereConcediuFaraPlataRegisterResponse> RegisterAsync(CerereConcediuFaraPlataRegisterRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/RegisterUnpaidLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataRegisterResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataRegisterResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataRegisterResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
    
    public async Task<CerereConcediuFaraPlataSendToEsignResponse> SendToEsignAsync(CerereConcediuFaraPlataSendToEsignRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/SendUnpaidLeaveRequestToEsignExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataSendToEsignResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataSendToEsignResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataSendToEsignResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

    public async Task<CerereConcediuFaraPlataUploadSignedResponse> UploadSignedAsync(CerereConcediuFaraPlataUploadSignedRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/UploadSignedUnpaidLeaveRequestExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataUploadSignedResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataUploadSignedResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataUploadSignedResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }
    
    public async Task<CerereConcediuFaraPlataSendForApprovalResponse> SendForApprovalAsync(CerereConcediuFaraPlataSendForApprovalRequest req, CancellationToken ct = default)
    {
        using var resp = await http.PostAsJsonAsync(
            "vault/extensionmethod/SendUnpaidLeaveRequestForApprovalExtensionMethod",
            req, JsonOptions, ct);

        var content = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            return new CerereConcediuFaraPlataSendForApprovalResponse { Succes = false, Mesaj = $"MFWS {(int)resp.StatusCode}: {content}" };

        return JsonSerializer.Deserialize<CerereConcediuFaraPlataSendForApprovalResponse>(content, JsonOptions)
               ?? new CerereConcediuFaraPlataSendForApprovalResponse { Succes = false, Mesaj = "Răspuns gol/invalid de la VEM." };
    }

}








