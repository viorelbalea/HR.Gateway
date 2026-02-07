namespace HR.Gateway.Application.Models.CerereConcediuLaEveniment;

public sealed class CerereConcediuLaEvenimentGetByIdResponse
{
    public int Id { get; set; }
    public DateTime? DataCerere { get; set; }
    public DateTime? DataInceput { get; set; }
    public int? NumarZile { get; set; }
    public string TipEveniment { get; set; } = string.Empty;
    public int TipEvenimentId { get; set; }
    public string Inlocuitor { get; set; } = string.Empty;
    public string NumarInregistrare { get; set; } = string.Empty;
    public DateTime? DataInregistrare { get; set; }
    public string State { get; set; } = string.Empty;
}
