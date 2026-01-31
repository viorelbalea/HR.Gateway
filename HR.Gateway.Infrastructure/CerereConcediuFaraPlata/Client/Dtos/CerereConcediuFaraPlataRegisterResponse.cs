using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public class CerereConcediuFaraPlataRegisterResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; init; }
    [JsonPropertyName("Message")] public string? Mesaj { get; init; }

    [JsonPropertyName("NumarInregistrare")] public string? NumarInregistrare { get; init; }
    [JsonPropertyName("DataInregistrare")] public DateTime? DataInregistrare { get; init; }
}
