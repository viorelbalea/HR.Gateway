using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaRegisterRequest
{
    [JsonPropertyName("CerereConcediuOdihnaId")]
    public int CerereConcediuOdihnaId { get; init; }
}

