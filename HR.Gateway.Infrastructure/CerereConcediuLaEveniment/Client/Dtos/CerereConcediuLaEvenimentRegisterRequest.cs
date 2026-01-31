using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentRegisterRequest
{
    [JsonPropertyName("CerereConcediuEvenimentId")]
    public int CerereConcediuEvenimentId { get; set; }
}
