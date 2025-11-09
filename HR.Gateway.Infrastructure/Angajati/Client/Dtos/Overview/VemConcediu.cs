using System.Text.Json.Serialization;

namespace HR.Gateway.Infrastructure.Angajati.Client.Dtos.Overview;

public sealed class VemConcediu
{
    [JsonPropertyName("An")]   
    public int An { get; set; }
    
    [JsonPropertyName("AnId")] 
    public int AnId { get; set; }

    [JsonPropertyName("NumarZileAlocate")]   
    public int NumarZileAlocate   { get; set; }
    
    [JsonPropertyName("NumarZileConsumate")] 
    public int NumarZileConsumate { get; set; }
    
    [JsonPropertyName("NumarZileRamase")]    
    public int NumarZileRamase    { get; set; }
}