namespace HR.Gateway.Application.Models.CerereConcediuOdihna;

public class CerereConcediuOdihnaAllocateDaysRequest
{
    public required int CerereConcediuId { get; init; }
    public required IReadOnlyList<CerereConcediuOdihnaAllocateDaysItem> AlocariZileConcediu { get; init; }
        = Array.Empty<CerereConcediuOdihnaAllocateDaysItem>();
}

