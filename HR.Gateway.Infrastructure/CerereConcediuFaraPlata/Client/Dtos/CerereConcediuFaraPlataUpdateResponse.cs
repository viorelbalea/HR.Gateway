using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataUpdateResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; init; }
    [JsonPropertyName("Message")] public string? Mesaj { get; init; }
}

