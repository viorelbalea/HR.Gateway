using System.Text.Json.Serialization;

namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public class CerereConcediuOdihnaAllocateDaysRequest
{
    public int CerereConcediuId { get; init; }
    
    [JsonPropertyName("AlocariZileConcediu")]
    public List<CerereConcediuOdihnaAllocateDaysItem> AlocariZileLaCerere { get; init; } = new();
} 