using System.Text.Json.Serialization;
using HR.Gateway.Infrastructure.Angajat.Client.Dtos.Profil;

namespace HR.Gateway.Infrastructure.Angajat.Client.Dtos.Cereri;

public sealed class VemRaspunsCereriConcediuOdihna
{
    [JsonPropertyName("FoundSuccess")]         
    public bool Gasit { get; set; }
    
    [JsonPropertyName("AngajatModel")]         
    public VemProfilAngajat? Angajat { get; set; }
    
    [JsonPropertyName("CereriConcediuOdihna")] 
    public List<VemCerereConcediuOdihna>? Cereri { get; set; }

    [JsonPropertyName("CereriConcediuFaraPlata")]
    public List<VemCerereConcediuOdihna>? CereriFaraPlata { get; set; }

    [JsonPropertyName("CereriConcediuEveniment")]
    public List<VemCerereConcediuOdihna>? CereriEveniment { get; set; }
}
