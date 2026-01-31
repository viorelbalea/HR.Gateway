using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Overview;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Cereri;

namespace HR.Gateway.Infrastructure.Angajat.Client;

public interface IVemAngajatClient
{
    Task<VemRaspunsOverview?>               GetByEmailAsync(string email, CancellationToken ct);
    Task<VemRaspunsCereriConcediuOdihna?>   GetHolidayRequestsByEmailAsync(string email, CancellationToken ct);
    Task<VemRaspunsCereriConcediuOdihna?>   GetUnpaidLeaveRequestsByEmailAsync(string email, CancellationToken ct);
    Task<VemRaspunsCereriConcediuOdihna?>   GetSpecialEventLeaveRequestsByEmailAsync(string email, CancellationToken ct);
}
