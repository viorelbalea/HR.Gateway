namespace HR.Gateway.Application.Models.Concedii;

public sealed class ConcediuCalculateDaysResponse
{
    public required int NumarZile { get; init; }
    public IReadOnlyList<ConcediuYearDetailItem> PeAnDetaliat { get; init; } = Array.Empty<ConcediuYearDetailItem>();
}