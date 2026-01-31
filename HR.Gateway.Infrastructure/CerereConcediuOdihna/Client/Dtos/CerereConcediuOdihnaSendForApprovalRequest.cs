using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaSendForApprovalRequest
{
    [JsonPropertyName("CerereConcediuOdihnaId")]
    public required int CerereConcediuOdihnaId { get; init; }
}

