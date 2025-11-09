using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Application.Models.Concedii;
using HR.Gateway.Infrastructure.Concedii.Client;
using HR.Gateway.Infrastructure.Concedii.Client.Dtos;

namespace HR.Gateway.Infrastructure.Concedii.Services;

internal sealed class ConcediiCalculator(IVemConcediiService vem) : IConcediiCalculator
{
    public async Task<CalculeazaZileResult> CalculeazaZileAsync(
        string email, DateOnly dataInceput, DateOnly dataSfarsit, CancellationToken ct = default)
    {
        var start = DateTime.SpecifyKind(dataInceput.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
        var end   = DateTime.SpecifyKind(dataSfarsit .ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);

        var payload = await vem.CalculeazaZileAsync(email.Trim().ToLowerInvariant(), start, end, ct);

        var list = (payload.Holidays ?? new())
            .OrderByDescending(h => h.An)
            .Select(h => new ConcediuPeAnDetaliat
            {
                An        = h.An,
                AnId      = h.AnId,
                Alocate   = h.NumarZileAlocate,
                Consumate = h.NumarZileConsumate,
                Ramase    = h.NumarZileRamase
            })
            .ToList();

        return new CalculeazaZileResult
        {
            NumarZile    = payload.NumarZileCalculate,
            PeAnDetaliat = list
        };
    }
}