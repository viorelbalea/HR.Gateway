using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;

public class CerereConcediuOdihnaUploadSignedRequest
{
    [JsonPropertyName("CerereConcediuOdihnaId")]
    public int CerereConcediuOdihnaId { get; init; }

    [JsonPropertyName("FileName")]
    public string NumeFisier { get; init; } = string.Empty;

    // PDF in base64
    [JsonPropertyName("FileContentBase64")]
    public string ContinutFisierBase64 { get; init; } = string.Empty;
}

