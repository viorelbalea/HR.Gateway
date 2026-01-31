using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaGetReplacementsRequest
{
    [JsonPropertyName("Email")]
    public required string Email { get; init; }
}

