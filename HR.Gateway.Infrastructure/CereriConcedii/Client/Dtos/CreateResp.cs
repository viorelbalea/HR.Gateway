using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CereriConcedii.Client.Dtos;

public sealed class CreateResp
{
    [JsonPropertyName("Success")]                 public bool Success { get; init; }
    [JsonPropertyName("Message")]                 public string? Message { get; init; }
    [JsonPropertyName("CerereConcediuOdihnaId")]  public int? CerereConcediuOdihnaId { get; init; }
}