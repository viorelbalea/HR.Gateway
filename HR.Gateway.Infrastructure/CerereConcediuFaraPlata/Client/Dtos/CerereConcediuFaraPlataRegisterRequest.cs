using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public class CerereConcediuFaraPlataRegisterRequest
{
    [JsonPropertyName("CerereConcediuFaraPlataId")]
    public int CerereConcediuFaraPlataId { get; init; }
}
