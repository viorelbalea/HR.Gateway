namespace HR.Gateway.Application.Models.CerereConcediuLaEveniment;

public sealed class CerereConcediuLaEvenimentUpdateRequest
{
    public int CerereId { get; init; }
    public required string Email { get; init; }
    public required DateTime DataInceput { get; init; }
    public required int TipEvenimentId { get; init; }
    public required string EmailInlocuitor { get; init; }

    public int NumarZile { get; init; }
}
