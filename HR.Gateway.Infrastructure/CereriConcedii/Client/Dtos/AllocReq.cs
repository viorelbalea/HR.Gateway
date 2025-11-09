using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CereriConcedii.Client.Dtos;

public sealed class AllocReq
{
    [JsonPropertyName("CerereConcediuId")] public required int CerereConcediuId { get; init; }

    // atenție: VEM așteaptă "AlocariZileConcediu" exact așa
    [JsonPropertyName("AlocariZileConcediu")] public List<AllocItem> Items { get; init; } = new();

    public sealed class AllocItem
    {
        [JsonPropertyName("ConcediuPerAngajatId")] public required int ConcediuPerAngajatId { get; init; }
        [JsonPropertyName("NumarZile")]            public required int NumarZile { get; init; }
    }
}