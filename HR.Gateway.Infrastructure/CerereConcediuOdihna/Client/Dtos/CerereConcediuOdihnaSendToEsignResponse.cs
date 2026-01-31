using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaSendToEsignResponse
{
    [JsonPropertyName("Success")]
    public bool Succes { get; init; }

    [JsonPropertyName("Message")]
    public string? Mesaj { get; init; }
}

