using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public sealed class CerereConcediuOdihnaCreateRequest
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

    [JsonPropertyName("AlocareManuala")]
    public required bool AlocareManuala { get; init; }

    // Lista pe care o va primi VEM-ul in noul tip de request.
    [JsonPropertyName("AlocariZileConcediu")]
    public List<CerereConcediuOdihnaAllocateDaysItem> AlocariZileConcediu { get; init; } = new();
}

