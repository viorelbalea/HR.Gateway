using HR.Gateway.Application.Models.CerereConcediu;
using HR.Gateway.Application.Models.CerereConcediuFaraPlata;

namespace HR.Gateway.Application.Abstractions.CerereConcediuFaraPlata;

public interface ICerereConcediuFaraPlataReader
{
    Task<CerereConcediuFaraPlataGetByIdResponse?> GetByIdAsync(int cerereId, CancellationToken ct = default);
    Task<CerereConcediuGetDocumentResponse?> GetDocumentAsync(int mfilesObjectId, CancellationToken ct = default);
}


