using HR.Gateway.Api.Contracts.Angajati;
using HR.Gateway.Api.Contracts.Concedii.Common;

namespace HR.Gateway.Api.Contracts.Concedii.Common;

public sealed class AngajatGetOverviewResponse
{
    public required int TotalDisponibile { get; init; }
    public IReadOnlyList<ConcediuYearDetailItem>? PeAniDetaliat { get; init; }
}