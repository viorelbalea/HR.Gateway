using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.Angajati.Client.Dtos.Profil;

public sealed class VemProfilAngajat
{
    [JsonPropertyName("Nume")]        
    public string? Nume { get; set; }
    
    [JsonPropertyName("Prenume")]    
    public string? Prenume { get; set; }
    
    [JsonPropertyName("NumeComplet")] 
    public string? NumeComplet { get; set; }
    
    [JsonPropertyName("Email")]       
    public string? Email { get; set; }
    
    [JsonPropertyName("Departament")] 
    public string? Departament { get; set; }
    
    [JsonPropertyName("Functie")]     
    public string? Functie { get; set; }
    
    [JsonPropertyName("ZileConcediuRamase")] 
    public int? ZileConcediuRamase { get; set; }
}