using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Application.Models.Concedii;
using HR.Gateway.Infrastructure.Concediu.Client;
using HR.Gateway.Infrastructure.Concediu.Client.Dtos;

namespace HR.Gateway.Infrastructure.Concediu.Services;

internal sealed class ConcediuCalculator(IVemConcediuService vem) : IConcediuCalculator
{
    public async Task<ConcediuCalculateDaysResponse> CalculeazaZileAsync(
        string email, DateOnly dataInceput, DateOnly dataSfarsit, CancellationToken ct = default)
    {
        var start = DateTime.SpecifyKind(dataInceput.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
        var end   = DateTime.SpecifyKind(dataSfarsit .ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);

        var payload = await vem.CalculeazaZileAsync(email.Trim().ToLowerInvariant(), start, end, ct);

        var list = (payload.Holidays ?? new())
            .OrderByDescending(h => h.An)
            .Select(h => new ConcediuYearDetailItem
            {
                An        = h.An,
                AnId      = h.AnId,
                Alocate   = h.NumarZileAlocate,
                Consumate = h.NumarZileConsumate,
                Ramase    = h.NumarZileRamase
            })
            .ToList();

        return new ConcediuCalculateDaysResponse
        {
            NumarZile    = payload.NumarZileCalculate,
            PeAnDetaliat = list
        };
    }
}
