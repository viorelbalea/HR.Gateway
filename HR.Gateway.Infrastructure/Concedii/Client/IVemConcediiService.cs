using HR.Gateway.Infrastructure.Concedii.Client.Dtos;

public interface IVemConcediiService
{
    Task<VemCalculeazaZileResponse> CalculeazaZileAsync(
        string email, DateTime start, DateTime end, CancellationToken ct);
}