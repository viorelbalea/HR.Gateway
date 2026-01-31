using HR.Gateway.Application.Models.CerereConcediuFaraPlata;

namespace HR.Gateway.Application.Abstractions.CerereConcediuFaraPlata;

public interface ICerereConcediuFaraPlataWriter
{
    Task<int> CreeazaCerereAsync(CerereConcediuFaraPlataCreateRequest req, CancellationToken ct = default);
    Task ActualizeazaCerereAsync(CerereConcediuFaraPlataUpdateRequest req, CancellationToken ct = default);
    Task InregistreazaCerereAsync(int cerereId, CancellationToken ct = default);
    Task TrimiteInSemnareElectronicaAsync(int cerereId, CancellationToken ct = default);
    Task IncarcaDocumentSemnatAsync(int cerereId, string fileName, Stream content, CancellationToken ct = default);
    Task TrimiteInAprobareAvizareAsync(int cerereId, CancellationToken ct = default);
}




