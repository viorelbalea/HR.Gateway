using HR.Gateway.Application.Models.CereriConcediu;

namespace HR.Gateway.Application.Abstractions.Concedii;

public interface ICereriConcediiWriter
{
    /// Creează documentul cererii în M-Files (VEM #1) și întoarce ID-ul M-Files.
    Task<int> CreeazaCerereAsync(CreareCerereConcediuOdihnaReq req, CancellationToken ct = default);

    /// Alocă zilele (VEM #2) pentru cererea creată anterior.
    Task AlocaZileAsync(AlocareZileLaCerereConcediuOdihnaReq req, CancellationToken ct = default);
}