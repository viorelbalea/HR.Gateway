namespace HR.Gateway.Application.Models.CerereConcediuOdihna;

public class CerereConcediuOdihnaCreateRequest
{
    public required string Email { get; init; }
    public required DateTime DataInceput { get; init; }
    public required DateTime DataSfarsit { get; init; }
    public required string EmailInlocuitor { get; init; }
    public int NumarZileCalculate { get; init; }
    public bool AlocareManuala { get; init; }

    public required IReadOnlyList<CerereConcediuOdihnaAllocateDaysItem> AlocariZileConcediu
    { get; init; } = Array.Empty<CerereConcediuOdihnaAllocateDaysItem>();
}



