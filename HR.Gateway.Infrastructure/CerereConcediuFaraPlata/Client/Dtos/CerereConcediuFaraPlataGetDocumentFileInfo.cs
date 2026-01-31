using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataGetDocumentFileInfo
{
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonPropertyName("Title")]
    public string? Titlu { get; set; }

    [JsonPropertyName("Extension")]
    public string? Extensie { get; set; }
}
