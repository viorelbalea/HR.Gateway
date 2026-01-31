namespace HR.Gateway.Api.Contracts.Concedii.ConcediuLaEveniment;

public sealed class CerereConcediuLaEvenimentCreateResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public int CerereConcediuEvenimentId { get; init; }
}
