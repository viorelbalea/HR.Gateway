using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataCreateRequest
{
    [JsonPropertyName("Email")]
    public required string Email { get; init; }

    [JsonPropertyName("DataInceput")]
    public required DateTime DataInceput { get; init; }

    [JsonPropertyName("DataSfarsit")]
    public required DateTime DataSfarsit { get; init; }

    [JsonPropertyName("EmailInlocuitor")]
    public required string EmailInlocuitor { get; init; }

    [JsonPropertyName("NumarZileCalculate")]
    public required int NumarZileCalculate { get; init; }

    [JsonPropertyName("MotivConcediu")]
    public string? Motiv { get; init; }
}
