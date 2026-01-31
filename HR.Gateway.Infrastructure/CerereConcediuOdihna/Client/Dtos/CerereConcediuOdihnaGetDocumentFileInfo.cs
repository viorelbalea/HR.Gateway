using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public sealed class CerereConcediuOdihnaGetDocumentFileInfo
{
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonPropertyName("Title")]
    public string? Titlu { get; set; }

    [JsonPropertyName("Extension")]
    public string? Extensie { get; set; }
}

