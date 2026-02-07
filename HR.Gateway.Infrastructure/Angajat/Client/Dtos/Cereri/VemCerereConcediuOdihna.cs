using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.Angajat.Client.Dtos.Cereri;

public sealed class VemCerereConcediuOdihna
{
    [JsonPropertyName("Id")]                 
    public int Id { get; set; }
    
    [JsonPropertyName("DataCerere")]         
    public DateTime? DataCerere { get; set; }
    
    [JsonPropertyName("DataInceput")]        
    public DateTime? DataInceput { get; set; }
    
    [JsonPropertyName("DataSfarsit")]       
    public DateTime? DataSfarsit { get; set; }
    
    [JsonPropertyName("NumarZile")]          
    public int? NumarZile { get; set; }
    
    [JsonPropertyName("Inlocuitor")]         
    public string Inlocuitor { get; set; } = "";
    
    [JsonPropertyName("NumarInregistrare")]  
    public string NumarInregistrare { get; set; } = "";
    
    [JsonPropertyName("DataInregistrare")]   
    public DateTime? DataInregistrare { get; set; }
    
    [JsonPropertyName("State")]
    public string Stare { get; set; } = "";

    // Proprietăți adiționale pentru Concediu Fără Plată
    [JsonPropertyName("MotivConcediu")]
    public string? MotivConcediu { get; set; }

    // Proprietăți adiționale pentru Concediu La Eveniment
    [JsonPropertyName("TipEveniment")]
    public string? TipEveniment { get; set; }

    [JsonPropertyName("TipEvenimentId")]
    public int? TipEvenimentId { get; set; }
}
