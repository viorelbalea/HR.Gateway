using HR.Gateway.Application.Models.Concedii;

namespace HR.Gateway.Application.Models.CalculatorConcedii;

public sealed class CalculatorConcedii
{
    public required int NumarZile { get; init; }
    public required IReadOnlyList<ConcediuPeAnDetaliat> PeAnDetaliat { get; init; }
}