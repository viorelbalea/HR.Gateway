using HR.Gateway.Infrastructure.Angajati.Client.Dtos.Overview;
using HR.Gateway.Infrastructure.Angajati.Client.Dtos.Cereri;

namespace HR.Gateway.Infrastructure.Angajati.Client;

public interface IVemAngajatiClient
{
    Task<VemRaspunsOverview?>               GetByEmailAsync(string email, CancellationToken ct);
    Task<VemRaspunsCereriConcediuOdihna?>   GetHolidayRequestsByEmailAsync(string email, CancellationToken ct);
}