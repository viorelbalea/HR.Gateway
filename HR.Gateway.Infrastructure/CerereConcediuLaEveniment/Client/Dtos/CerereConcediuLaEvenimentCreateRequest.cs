using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentCreateRequest
{
    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("DataInceput")]
    public DateTime DataInceput { get; set; }

    [JsonPropertyName("TipEvenimentId")]
    public int TipEvenimentId { get; set; }

    [JsonPropertyName("NumarZile")]
    public int? NumarZile { get; set; }

    [JsonPropertyName("EmailInlocuitor")]
    public string EmailInlocuitor { get; set; } = string.Empty;

}
