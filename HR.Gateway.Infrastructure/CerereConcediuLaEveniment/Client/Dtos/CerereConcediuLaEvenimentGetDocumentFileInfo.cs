using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentGetDocumentFileInfo
{
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonPropertyName("Title")]
    public string? Titlu { get; set; }

    [JsonPropertyName("Extension")]
    public string? Extensie { get; set; }
}
