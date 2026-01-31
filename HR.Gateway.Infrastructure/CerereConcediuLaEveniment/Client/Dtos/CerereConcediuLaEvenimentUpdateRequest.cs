using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentUpdateRequest
{
    [JsonPropertyName("CerereConcediuEvenimentId")]
    public int CerereConcediuEvenimentId { get; set; }

    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("DataInceput")]
    public DateTime DataInceput { get; set; }

    [JsonPropertyName("DataSfarsit")]
    public DateTime DataSfarsit { get; set; }

    [JsonPropertyName("EmailInlocuitor")]
    public string EmailInlocuitor { get; set; } = string.Empty;

    [JsonPropertyName("NumarZileCalculate")]
    public int NumarZileCalculate { get; set; }

    [JsonPropertyName("Motiv")]
    public string Motiv { get; set; } = string.Empty;
}
