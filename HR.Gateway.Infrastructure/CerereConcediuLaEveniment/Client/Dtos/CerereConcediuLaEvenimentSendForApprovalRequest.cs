using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentSendForApprovalRequest
{
    [JsonPropertyName("CerereConcediuEvenimentId")]
    public int CerereConcediuEvenimentId { get; init; }
}
