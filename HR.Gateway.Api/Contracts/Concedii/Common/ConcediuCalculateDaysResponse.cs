namespace HR.Gateway.Api.Contracts.Concedii.Common;

public sealed class ConcediuCalculateDaysResponse
{
    public required int NumarZile { get; init; }
    public IReadOnlyList<ConcediuYearDetailItem> PeAnDetaliat { get; init; } = new List<ConcediuYearDetailItem>();
}