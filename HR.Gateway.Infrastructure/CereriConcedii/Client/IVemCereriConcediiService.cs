using HR.Gateway.Infrastructure.CereriConcedii.Client.Dtos;

namespace HR.Gateway.Infrastructure.CereriConcedii.Client;

public interface IVemCereriConcediiService
{
    Task<CreateResp> CreateAsync(CreateReq req, CancellationToken ct);
    Task<AllocResp>  AllocateAsync(AllocReq req, CancellationToken ct);
}