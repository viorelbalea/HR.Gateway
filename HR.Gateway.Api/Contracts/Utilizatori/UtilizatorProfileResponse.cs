using System.Text.Json.Serialization;

namespace HR.Gateway.Api.Contracts.Utilizatori;

public sealed class UtilizatorProfileResponse
{
    public required string Email { get; init; }
    
    [JsonPropertyName("fullName")]
    public required string NumeComplet { get; init; }
    
    [JsonPropertyName("firstName")]
    public string? Prenume { get; init; }  
    
    [JsonPropertyName("lastName")]
    public string? Nume    { get; init; }  
    
    [JsonPropertyName("department")]
    public string? Departament { get; init; }
    
    [JsonPropertyName("position")]
    public string? Functie     { get; init; }
}