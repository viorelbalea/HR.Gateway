using HR.Gateway.Application.Models.CerereConcediuLaEveniment;

namespace HR.Gateway.Application.Abstractions.CerereConcediuLaEveniment;

public interface ICerereConcediuLaEvenimentWriter
{
    Task<int> CreeazaCerereAsync(CerereConcediuLaEvenimentCreateRequest req, CancellationToken ct = default);
    Task ActualizeazaCerereAsync(CerereConcediuLaEvenimentUpdateRequest req, CancellationToken ct = default);
    Task InregistreazaCerereAsync(int cerereId, CancellationToken ct = default);
    Task TrimiteInSemnareElectronicaAsync(int cerereId, CancellationToken ct = default);
    Task IncarcaDocumentSemnatAsync(int cerereId, string fileName, Stream content, CancellationToken ct = default);
    Task TrimiteInAprobareAvizareAsync(int cerereId, CancellationToken ct = default);
}

