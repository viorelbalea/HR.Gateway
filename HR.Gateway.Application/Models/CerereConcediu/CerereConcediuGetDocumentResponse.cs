namespace HR.Gateway.Application.Models.CerereConcediu;

public sealed class CerereConcediuGetDocumentResponse
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}


