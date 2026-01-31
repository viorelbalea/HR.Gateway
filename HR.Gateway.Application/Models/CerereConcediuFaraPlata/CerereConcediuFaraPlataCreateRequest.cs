namespace HR.Gateway.Application.Models.CerereConcediuFaraPlata;

public sealed class CerereConcediuFaraPlataCreateRequest
{
    public required string Email { get; init; }
    public required DateTime DataInceput { get; init; }
    public required DateTime DataSfarsit { get; init; }
    public required string EmailInlocuitor { get; init; }

    public int NumarZileCalculate { get; init; }
    public string? Motiv { get; init; }
}


