using System.Text.Json.Serialization;
using HR.Gateway.Infrastructure.Angajati.Client.Dtos.Profil;

namespace HR.Gateway.Infrastructure.Angajati.Client.Dtos.Cereri;

public sealed class VemRaspunsCereriConcediuOdihna
{
    [JsonPropertyName("FoundSuccess")]         
    public bool Gasit { get; set; }
    
    [JsonPropertyName("AngajatModel")]         
    public VemProfilAngajat? Angajat { get; set; }
    
    [JsonPropertyName("CereriConcediuOdihna")] 
    public List<VemCerereConcediuOdihna>? Cereri { get; set; }
}