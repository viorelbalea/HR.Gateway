namespace HR.Gateway.Application.Models.CerereConcediuOdihna;

public class CerereConcediuOdihnaUpdateRequest
{
    public int CerereId { get; init; }

    public DateTime DataInceput { get; init; }
    public DateTime DataSfarsit { get; init; }
    public string? EmailInlocuitor { get; init; }
    public string? Motiv { get; init; }
    
    public int NumarZileCalculate { get; init; }
    public bool AlocareManuala { get; init; }

    public CerereConcediuOdihnaAllocateDaysItem[] AlocariZileConcediu { get; init; }
        = Array.Empty<CerereConcediuOdihnaAllocateDaysItem>();
    
}



