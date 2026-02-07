using System.Text.Json.Serialization;
using HR.Gateway.Infrastructure.Common;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentGetByIdResponse
{
    [JsonPropertyName("FoundSuccess")] public bool Succes { get; set; }
    [JsonPropertyName("Message")] public string? Mesaj { get; set; }

    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("DataCerere")]
    public DateTime? DataCerere { get; set; }

    [JsonPropertyName("DataInceput")]
    public DateTime? DataInceput { get; set; }

    [JsonPropertyName("NumarZile")]
    [JsonConverter(typeof(NullableIntConverter))]
    public int? NumarZile { get; set; }

    [JsonPropertyName("TipEveniment")]
    public string TipEveniment { get; set; } = string.Empty;

    [JsonPropertyName("TipEvenimentId")]
    public int TipEvenimentId { get; set; }

    [JsonPropertyName("Inlocuitor")]
    public string Inlocuitor { get; set; } = string.Empty;

    [JsonPropertyName("NumarInregistrare")]
    public string NumarInregistrare { get; set; } = string.Empty;

    [JsonPropertyName("DataInregistrare")]
    public DateTime? DataInregistrare { get; set; }

    [JsonPropertyName("State")]
    public string State { get; set; } = string.Empty;
}
