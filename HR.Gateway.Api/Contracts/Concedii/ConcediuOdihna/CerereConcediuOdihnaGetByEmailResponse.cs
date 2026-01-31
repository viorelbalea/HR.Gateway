using HR.Gateway.Api.Contracts.Angajati;

namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public sealed class CerereConcediuOdihnaGetByEmailResponse
{
    public bool Gasit { get; set; }
    public AngajatProfileResponse? Angajat { get; set; }
    public List<CerereConcediuOdihnaGetByIdResponse> Cereri { get; set; } = new();
}