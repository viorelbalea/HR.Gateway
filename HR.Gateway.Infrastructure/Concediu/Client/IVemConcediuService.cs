using HR.Gateway.Infrastructure.Concediu.Client.Dtos;

namespace HR.Gateway.Infrastructure.Concediu.Client;

public interface IVemConcediuService
{
    Task<VemCalculeazaZileResponse> CalculeazaZileAsync(
        string email, DateTime start, DateTime end, CancellationToken ct);
}

