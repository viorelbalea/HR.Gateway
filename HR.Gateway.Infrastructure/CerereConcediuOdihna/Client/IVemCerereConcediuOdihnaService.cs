using HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client;

public interface IVemCerereConcediuOdihnaService
{
    Task<CerereConcediuOdihnaCreateResponse> CreateAsync(CerereConcediuOdihnaCreateRequest req, CancellationToken ct);
    Task<CerereConcediuOdihnaCreateResponse> UpdateAsync(CerereConcediuOdihnaUpdateRequest req, CancellationToken ct);
    Task<CerereConcediuOdihnaGetReplacementsResponse> GetReplacementsAsync(CerereConcediuOdihnaGetReplacementsRequest req, CancellationToken ct);
    Task<CerereConcediuOdihnaSendForApprovalResponse> SendForApprovalAsync(CerereConcediuOdihnaSendForApprovalRequest req, CancellationToken ct = default);
    Task<CerereConcediuOdihnaRegisterResponse> RegisterAsync(CerereConcediuOdihnaRegisterRequest req, CancellationToken ct = default);
    Task<CerereConcediuOdihnaSendToEsignResponse> SendToEsignAsync(CerereConcediuOdihnaSendToEsignRequest req, CancellationToken ct = default);
    Task<CerereConcediuOdihnaUploadSignedResponse> UploadSignedAsync(CerereConcediuOdihnaUploadSignedRequest req, CancellationToken ct = default);
    Task<CerereConcediuOdihnaGetByIdResponse?> GetByIdAsync(CerereConcediuOdihnaGetByIdRequest req, CancellationToken ct = default);
}



