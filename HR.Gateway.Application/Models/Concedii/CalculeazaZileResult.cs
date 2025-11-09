namespace HR.Gateway.Application.Models.Concedii;

public sealed class CalculeazaZileResult
{
    public required int NumarZile { get; init; }
    public IReadOnlyList<ConcediuPeAnDetaliat> PeAnDetaliat { get; init; } = Array.Empty<ConcediuPeAnDetaliat>();
}