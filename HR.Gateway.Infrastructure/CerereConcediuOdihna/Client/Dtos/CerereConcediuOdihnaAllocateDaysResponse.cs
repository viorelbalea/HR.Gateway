using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public sealed class CerereConcediuOdihnaAllocateDaysResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; init; }
    [JsonPropertyName("Message")] public string? Mesaj { get; init; }
}

