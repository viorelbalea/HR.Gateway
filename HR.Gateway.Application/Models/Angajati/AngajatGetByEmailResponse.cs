namespace HR.Gateway.Application.Models.Angajati;

public sealed class AngajatGetByEmailResponse
{
    public required string Email { get; init; }
    public required string NumeComplet { get; init; }
    public string? Prenume { get; set; }    
    public string? Nume    { get; set; }  
    public string? Departament { get; init; }
    public string? Functie     { get; init; }
}