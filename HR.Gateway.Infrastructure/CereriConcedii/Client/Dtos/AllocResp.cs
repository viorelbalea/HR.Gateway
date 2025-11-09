using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CereriConcedii.Client.Dtos;

public sealed class AllocResp
{
    [JsonPropertyName("Success")] public bool Success { get; init; }
    [JsonPropertyName("Message")] public string? Message { get; init; }
}