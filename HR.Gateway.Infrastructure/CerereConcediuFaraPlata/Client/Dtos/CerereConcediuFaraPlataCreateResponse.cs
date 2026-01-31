using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataCreateResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; init; }
    [JsonPropertyName("Message")] public string? Mesaj { get; init; }
    [JsonPropertyName("CerereConcediuFaraPlataId")] public int CerereConcediuFaraPlataId { get; init; }
}
