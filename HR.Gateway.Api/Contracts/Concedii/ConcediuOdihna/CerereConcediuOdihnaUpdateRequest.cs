namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public class CerereConcediuOdihnaUpdateRequest
{
    public DateTime DataInceput { get; init; }
    public DateTime DataSfarsit { get; init; }
    public string? EmailInlocuitor { get; init; }
    public string? Motiv { get; init; }
    
    public int NumarZileCalculate { get; init; }
    public bool AlocareManuala { get; init; }

    public List<CerereConcediuOdihnaAllocateDaysItem> AlocariZileLaCerere { get; init; } = new();
}