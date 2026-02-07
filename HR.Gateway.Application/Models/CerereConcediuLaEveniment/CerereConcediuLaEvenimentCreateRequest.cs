namespace HR.Gateway.Application.Models.CerereConcediuLaEveniment;

public sealed class CerereConcediuLaEvenimentCreateRequest
{
    public required string Email { get; init; }
    public required DateTime DataInceput { get; init; }
    public required int TipEvenimentId { get; init; }
    public int? NumarZile { get; init; }
    public required string EmailInlocuitor { get; init; }
    public int NumarZileCalculate { get; init; }
}
