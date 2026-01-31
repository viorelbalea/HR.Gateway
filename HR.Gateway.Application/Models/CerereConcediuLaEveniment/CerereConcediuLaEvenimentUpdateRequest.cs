namespace HR.Gateway.Application.Models.CerereConcediuLaEveniment;

public sealed class CerereConcediuLaEvenimentUpdateRequest
{
    public int CerereId { get; init; }
    public required string Email { get; init; }
    public required DateTime DataInceput { get; init; }
    public required DateTime DataSfarsit { get; init; }
    public required string EmailInlocuitor { get; init; }

    public int NumarZileCalculate { get; init; }
    public string? Motiv { get; init; }
}
