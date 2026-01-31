using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataUpdateRequest
{
    [JsonPropertyName("CerereConcediuFaraPlataId")]
    public int CerereConcediuFaraPlataId { get; init; }

    [JsonPropertyName("Email")]
    public required string Email { get; init; }

    [JsonPropertyName("DataInceput")]
    public DateTime DataInceput { get; init; }

    [JsonPropertyName("DataSfarsit")]
    public DateTime DataSfarsit { get; init; }

    [JsonPropertyName("EmailInlocuitor")]
    public required string EmailInlocuitor { get; init; }

    [JsonPropertyName("MotivConcediu")]
    public string? Motiv { get; init; }

    [JsonPropertyName("NumarZileCalculate")]
    public int NumarZileCalculate { get; init; }
}
