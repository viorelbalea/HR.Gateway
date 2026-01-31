using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaSendToEsignRequest
{
    [JsonPropertyName("CerereConcediuOdihnaId")]
    public int CerereConcediuOdihnaId { get; init; }
}

