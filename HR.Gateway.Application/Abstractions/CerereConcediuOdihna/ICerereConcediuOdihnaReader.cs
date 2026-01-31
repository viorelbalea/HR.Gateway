using HR.Gateway.Application.Models.CerereConcediu;
using HR.Gateway.Application.Models.CerereConcediuOdihna;

namespace HR.Gateway.Application.Abstractions.CerereConcediuOdihna;

public interface ICerereConcediuOdihnaReader
{
    Task<CerereConcediuGetDocumentResponse?> GetDocumentAsync(int cerereId, CancellationToken ct = default);

    // Returneaza detaliile cererii (pentru Polling ?i vizualizare)
    Task<CerereConcediuOdihnaGetByIdResponse?> GetByIdAsync(int cerereId, CancellationToken ct = default);
}


