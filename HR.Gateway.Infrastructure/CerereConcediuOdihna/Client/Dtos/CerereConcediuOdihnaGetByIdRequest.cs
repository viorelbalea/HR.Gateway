using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaGetByIdRequest
{
    [JsonPropertyName("CerereConcediuOdihnaId")]
    public int CerereConcediuOdihnaId { get; init; }
}

