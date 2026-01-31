using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

public sealed class CerereConcediuLaEvenimentUploadSignedRequest
{
    [JsonPropertyName("CerereConcediuEvenimentId")]
    public int CerereConcediuEvenimentId { get; init; }

    [JsonPropertyName("FileName")]
    public string NumeFisier { get; init; } = string.Empty;

    [JsonPropertyName("FileContentBase64")]
    public string ContinutFisierBase64 { get; init; } = string.Empty;
}
