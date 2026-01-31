using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaUpdateRequest
{
    [JsonPropertyName("CerereConcediuOdihnaId")]
    public int CerereConcediuOdihnaId { get; init; }

    [JsonPropertyName("DataInceput")]
    public DateTime DataInceput { get; init; }

    [JsonPropertyName("DataSfarsit")]
    public DateTime DataSfarsit { get; init; }

    [JsonPropertyName("EmailInlocuitor")]
    public string? EmailInlocuitor { get; init; }

    [JsonPropertyName("Motiv")]
    public string? Motiv { get; init; }

    [JsonPropertyName("NumarZileCalculate")]
    public int NumarZileCalculate { get; init; }

    [JsonPropertyName("AlocareManuala")]
    public bool AlocareManuala { get; init; }

    [JsonPropertyName("AlocariZileConcediu")]
    public List<CerereConcediuOdihnaAllocateDaysItem> AlocariZileConcediu { get; init; } = new();
}

