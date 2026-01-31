namespace HR.Gateway.Api.Contracts.Concedii.ConcediuFaraPlata;

public sealed class ActualizeazaCerereConcediuFaraPlataRequest
{
    // op?ional: daca vrei sa-l trimi?i explicit; altfel îl deducem din user logat
    public string? Email { get; init; }

    public required DateTime DataInceput { get; init; }
    public required DateTime DataSfarsit { get; init; }

    public required string EmailInlocuitor { get; init; }

    public int NumarZileCalculate { get; init; }

    public string? Motiv { get; init; }
}

