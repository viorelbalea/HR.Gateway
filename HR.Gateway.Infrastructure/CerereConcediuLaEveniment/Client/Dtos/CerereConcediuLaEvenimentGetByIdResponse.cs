using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentGetByIdResponse
{
    [JsonPropertyName("Success")] public bool Succes { get; set; }
    [JsonPropertyName("Message")] public string? Mesaj { get; set; }

    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("Created")]
    public DateTime DataCreare { get; set; }

    [JsonPropertyName("DataInceput")]
    public DateTime DataInceput { get; set; }

    [JsonPropertyName("DataSfarsit")]
    public DateTime DataSfarsit { get; set; }

    [JsonPropertyName("NumarZile")]
    public int NumarZile { get; set; }

    [JsonPropertyName("Stare")]
    public string Stare { get; set; } = string.Empty;

    [JsonPropertyName("InlocuitorNume")]
    public string Inlocuitor { get; set; } = string.Empty;

    [JsonPropertyName("NumarInregistrare")]
    public string? NumarInregistrare { get; set; }

    [JsonPropertyName("DataInregistrare")]
    public DateTime? DataInregistrare { get; set; }

    [JsonPropertyName("Motiv")]
    public string Motiv { get; set; } = string.Empty;
}
