using HR.Gateway.Application.Models.CerereConcediuOdihna;

namespace HR.Gateway.Application.Abstractions.CerereConcediuOdihna;

public interface ICerereConcediuOdihnaWriter
{
    Task<int> CreeazaCerereAsync(CerereConcediuOdihnaCreateRequest req, CancellationToken ct = default);

    Task ActualizeazaCerereAsync(CerereConcediuOdihnaUpdateRequest req, CancellationToken ct = default);
    
    Task InregistreazaCerereAsync(int cerereId, CancellationToken ct = default);
    
    Task TrimiteInSemnareElectronicaAsync(int cerereId, CancellationToken ct = default);
    
    Task IncarcaDocumentSemnatAsync(int cerereId, string fileName, Stream content, CancellationToken ct = default);
        
    Task TrimiteInAprobareAvizareAsync(int cerereId, CancellationToken ct = default);
}




