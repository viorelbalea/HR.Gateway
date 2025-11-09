using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.Angajati.Client.Dtos.Cereri;

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
}