namespace HR.Gateway.Application.Models.CerereConcediuFaraPlata;

public sealed class CerereConcediuFaraPlataUpdateRequest
{
    public int CerereId { get; init; }

    public required string Email { get; init; }
    public required string EmailInlocuitor { get; init; }

    public DateTime DataInceput { get; init; }
    public DateTime DataSfarsit { get; init; }

    public int NumarZileCalculate { get; init; }

    public string? Motiv { get; init; }
}


