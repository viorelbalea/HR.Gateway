using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaGetByIdResponse
{
    [JsonPropertyName("Id")]
    public int Id { get; init; }

    [JsonPropertyName("DataCerere")]
    public DateTime DataCreare { get; init; }

    [JsonPropertyName("DataInceput")]
    public DateTime DataInceput { get; init; }

    [JsonPropertyName("DataSfarsit")]
    public DateTime DataSfarsit { get; init; }

    [JsonPropertyName("NumarZile")]
    public int NumarZile { get; init; }

    [JsonPropertyName("State")]
    public string Stare { get; init; } = string.Empty;

    [JsonPropertyName("Inlocuitor")]
    public string Inlocuitor { get; init; } = string.Empty;

    [JsonPropertyName("NumarInregistrare")]
    public string? NumarInregistrare { get; init; }

    [JsonPropertyName("DataInregistrare")]
    public DateTime? DataInregistrare { get; init; }

    [JsonPropertyName("Success")]
    public bool Succes { get; init; } = true;

    [JsonPropertyName("Message")]
    public string? Mesaj { get; init; }
}

