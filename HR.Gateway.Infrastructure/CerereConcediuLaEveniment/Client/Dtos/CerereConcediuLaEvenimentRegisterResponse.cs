using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentRegisterResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; set; }
    [JsonPropertyName("Message")] public string? Mesaj { get; set; }
    [JsonPropertyName("NumarInregistrare")] public string NumarInregistrare { get; set; } = string.Empty;
    [JsonPropertyName("DataInregistrare")] public DateTime? DataInregistrare { get; set; }
}
