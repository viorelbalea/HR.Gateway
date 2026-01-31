using HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client;

public interface IVemCerereConcediuLaEvenimentService
{
    Task<CerereConcediuLaEvenimentCreateResponse> CreateAsync(CerereConcediuLaEvenimentCreateRequest req, CancellationToken ct = default);
    Task<CerereConcediuLaEvenimentUpdateResponse> UpdateAsync(CerereConcediuLaEvenimentUpdateRequest req, CancellationToken ct = default);
    Task<CerereConcediuLaEvenimentGetByIdResponse?> GetByIdAsync(CerereConcediuLaEvenimentGetByIdRequest req, CancellationToken ct = default);
    Task<CerereConcediuLaEvenimentRegisterResponse> RegisterAsync(CerereConcediuLaEvenimentRegisterRequest req, CancellationToken ct = default);
    Task<CerereConcediuLaEvenimentSendToEsignResponse> SendToEsignAsync(CerereConcediuLaEvenimentSendToEsignRequest req, CancellationToken ct = default);
    Task<CerereConcediuLaEvenimentUploadSignedResponse> UploadSignedAsync(CerereConcediuLaEvenimentUploadSignedRequest req, CancellationToken ct = default);
    Task<CerereConcediuLaEvenimentSendForApprovalResponse> SendForApprovalAsync(CerereConcediuLaEvenimentSendForApprovalRequest req, CancellationToken ct = default);
}







