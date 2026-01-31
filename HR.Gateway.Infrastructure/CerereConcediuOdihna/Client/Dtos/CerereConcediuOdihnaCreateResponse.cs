using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public sealed class CerereConcediuOdihnaCreateResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; init; }
    [JsonPropertyName("Message")] public string? Mesaj { get; init; }
    [JsonPropertyName("CerereConcediuOdihnaId")] public int? CerereConcediuOdihnaId { get; init; }
}

