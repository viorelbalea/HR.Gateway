using HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client;

public interface IVemCerereConcediuFaraPlataService
{
    Task<CerereConcediuFaraPlataCreateResponse> CreateAsync(CerereConcediuFaraPlataCreateRequest req, CancellationToken ct = default);
    Task<CerereConcediuFaraPlataUpdateResponse> UpdateAsync(CerereConcediuFaraPlataUpdateRequest req, CancellationToken ct = default);
    Task<CerereConcediuFaraPlataGetByIdResponse?> GetByIdAsync(CerereConcediuFaraPlataGetByIdRequest req, CancellationToken ct = default);
    Task<CerereConcediuFaraPlataRegisterResponse> RegisterAsync(CerereConcediuFaraPlataRegisterRequest req, CancellationToken ct = default);
    Task<CerereConcediuFaraPlataSendToEsignResponse> SendToEsignAsync(CerereConcediuFaraPlataSendToEsignRequest req, CancellationToken ct = default);
    Task<CerereConcediuFaraPlataUploadSignedResponse> UploadSignedAsync(CerereConcediuFaraPlataUploadSignedRequest req, CancellationToken ct = default);
    Task<CerereConcediuFaraPlataSendForApprovalResponse> SendForApprovalAsync(CerereConcediuFaraPlataSendForApprovalRequest req, CancellationToken ct = default);
}






