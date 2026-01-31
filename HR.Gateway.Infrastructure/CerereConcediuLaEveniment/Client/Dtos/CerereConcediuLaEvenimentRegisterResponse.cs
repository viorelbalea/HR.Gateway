using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentRegisterResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; set; }
    [JsonPropertyName("Message")] public string? Mesaj { get; set; }
}
