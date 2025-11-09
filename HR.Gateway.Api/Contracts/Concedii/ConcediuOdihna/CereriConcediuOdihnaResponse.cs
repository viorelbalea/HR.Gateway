using HR.Gateway.Api.Contracts.Angajati;

namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public sealed class CereriConcediuOdihnaResponse
{
    public bool Gasit { get; set; }
    public ProfilAngajatDto? Angajat { get; set; }
    public List<CerereConcediuOdihnaDto> Cereri { get; set; } = new();
}