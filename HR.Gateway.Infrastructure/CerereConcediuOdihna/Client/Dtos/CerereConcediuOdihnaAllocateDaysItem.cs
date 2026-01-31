using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public sealed class CerereConcediuOdihnaAllocateDaysItem
{
    [JsonPropertyName("ConcediuPerAngajatId")] public required int ConcediuPerAngajatId { get; init; }
    [JsonPropertyName("NumarZile")] public required int NumarZile { get; init; }
}

