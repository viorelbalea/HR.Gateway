namespace HR.Gateway.Application.Models.CerereConcediuOdihna;

public sealed class CerereConcediuOdihnaGetByIdResponse
{
    public int Id { get; set; }
    public DateTime DataCreare { get; set; }
    public DateTime DataInceput { get; set; }
    public DateTime DataSfarsit { get; set; }
    public int NumarZile { get; set; }
    
    // Proprietatea vitala pentru Polling
    public string Stare { get; set; } = string.Empty;
    
    public string Inlocuitor { get; set; } = string.Empty;
    public string? NumarInregistrare { get; set; }
    public DateTime? DataInregistrare { get; set; }
}
