using HR.Gateway.Application.Models.CerereConcediu;
using HR.Gateway.Application.Models.CerereConcediuLaEveniment;

namespace HR.Gateway.Application.Abstractions.CerereConcediuLaEveniment;

public interface ICerereConcediuLaEvenimentReader
{
    Task<CerereConcediuLaEvenimentGetByIdResponse?> GetByIdAsync(int cerereId, CancellationToken ct = default);
    Task<CerereConcediuGetDocumentResponse?> GetDocumentAsync(int id, CancellationToken ct = default);
}

