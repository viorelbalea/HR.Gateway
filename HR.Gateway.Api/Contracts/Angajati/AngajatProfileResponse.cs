namespace HR.Gateway.Api.Contracts.Angajati;

public sealed class AngajatProfileResponse
{
    public string Nume { get; set; } = "";
    public string Prenume { get; set; } = "";
    public string NumeComplet { get; set; } = "";
    public string Email { get; set; } = "";
    public string Departament { get; set; } = "";
    public string Functie { get; set; } = "";
    public int ZileConcediuRamase { get; set; }
}