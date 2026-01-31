using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaRegisterResponse
{
    [JsonPropertyName("Success")]
    public bool Succes { get; init; }

    [JsonPropertyName("Message")]
    public string? Mesaj { get; init; }

    [JsonPropertyName("NumarInregistrare")]
    public string? NumarInregistrare { get; init; }

    [JsonPropertyName("DataInregistrare")]
    public DateTime? DataInregistrare { get; init; }
}

