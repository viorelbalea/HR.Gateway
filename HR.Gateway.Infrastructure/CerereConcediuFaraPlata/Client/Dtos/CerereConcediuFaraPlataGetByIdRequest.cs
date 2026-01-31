using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataGetByIdRequest
{
    [JsonPropertyName("CerereConcediuFaraPlataId")]
    public int CerereConcediuFaraPlataId { get; init; }
}
