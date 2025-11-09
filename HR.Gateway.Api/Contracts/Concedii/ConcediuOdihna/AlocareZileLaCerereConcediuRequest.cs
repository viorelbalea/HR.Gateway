using System.Text.Json.Serialization;

namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public class AlocareZileLaCerereConcediuRequest
{
    public int CerereConcediuId { get; init; }
    
    [JsonPropertyName("AlocariZileConcediu")]
    public List<AlocareZileLaCerereConcediuDto> AlocariZileLaCerere { get; init; } = new();
} 