using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaGetReplacementsItem
{
    [JsonPropertyName("Email")]
    public string Email { get; init; } = "";

    [JsonPropertyName("NumeComplet")]
    public string NumeComplet { get; init; } = "";
}

