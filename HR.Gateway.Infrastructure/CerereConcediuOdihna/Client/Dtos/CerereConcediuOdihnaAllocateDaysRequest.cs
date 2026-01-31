using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public sealed class CerereConcediuOdihnaAllocateDaysRequest
{
    [JsonPropertyName("CerereConcediuId")] public required int CerereConcediuId { get; init; }

    // atentie: VEM asteapta "AlocariZileConcediu" exact asa
    [JsonPropertyName("AlocariZileConcediu")] public List<CerereConcediuOdihnaAllocateDaysItem> AlocariZileConcediu { get; init; } = new();
}

