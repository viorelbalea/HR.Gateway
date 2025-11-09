using System.Text.Json.Serialization;
using HR.Gateway.Infrastructure.Angajati.Client.Dtos.Profil;

namespace HR.Gateway.Infrastructure.Angajati.Client.Dtos.Overview;

public sealed class VemRaspunsOverview
{
    [JsonPropertyName("FoundSuccess")] 
    public bool Gasit { get; set; }
    
    [JsonPropertyName("AngajatModel")] 
    public VemProfilAngajat? Angajat { get; set; }
    
    [JsonPropertyName("Holidays")]     
    public List<VemConcediu>? Concedii { get; set; }
}