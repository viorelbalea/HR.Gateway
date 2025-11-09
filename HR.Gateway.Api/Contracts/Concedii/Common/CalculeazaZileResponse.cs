namespace HR.Gateway.Api.Contracts.Concedii.Common;

public sealed class CalculeazaZileResponse
{
    public required int NumarZile { get; init; }
    public IReadOnlyList<ConcediuPeAnDetaliatDto> PeAnDetaliat { get; init; } = new List<ConcediuPeAnDetaliatDto>();
}