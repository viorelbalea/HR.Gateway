using HR.Gateway.Application.Models.Concedii;

namespace HR.Gateway.Application.Abstractions.Concedii;

public interface IConcediiCalculator
{
    Task<CalculeazaZileResult> CalculeazaZileAsync(
        string email,
        DateOnly dataInceput,
        DateOnly dataSfarsit,
        CancellationToken ct = default);
}