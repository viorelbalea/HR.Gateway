namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public class CreareConcediuOdihnaRequest
{
    public string Email { get; init; } = default!;
    public DateTime DataInceput { get; init; }
    public DateTime DataSfarsit { get; init; }
    public string EmailInlocuitor { get; init; } = default!;
    public int NumarZileCalculate { get; init; }
    public bool AlocareManuala { get; init; }
}