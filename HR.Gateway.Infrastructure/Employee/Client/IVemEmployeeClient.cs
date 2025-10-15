namespace HR.Gateway.Infrastructure.Employee.Client;

internal interface IVemEmployeeClient
{
    Task<Dtos.VemAngajatResponse?> GetByEmailAsync(string email, CancellationToken ct);
}