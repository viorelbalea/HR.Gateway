using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

public sealed class CerereConcediuFaraPlataUploadSignedRequest
{
    [JsonPropertyName("CerereConcediuFaraPlataId")]
    public int CerereConcediuFaraPlataId { get; init; }

    [JsonPropertyName("FileName")]
    public string NumeFisier { get; init; } = string.Empty;

    [JsonPropertyName("FileContentBase64")]
    public string ContinutFisierBase64 { get; init; } = string.Empty;
}
