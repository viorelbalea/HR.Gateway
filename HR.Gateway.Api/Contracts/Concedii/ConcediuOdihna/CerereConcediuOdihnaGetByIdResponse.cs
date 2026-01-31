namespace HR.Gateway.Api.Contracts.Concedii.ConcediuOdihna;

public sealed class CerereConcediuOdihnaGetByIdResponse
{
    public int Id { get; set; }
    public DateTime? DataCerere { get; set; }
    public DateTime? DataInceput { get; set; }
    public DateTime? DataSfarsit { get; set; }
    public int? NumarZile { get; set; }
    public string Inlocuitor { get; set; } = "";
    public string NumarInregistrare { get; set; } = "";
    public DateTime? DataInregistrare { get; set; }
    public string Stare { get; set; } = "";
}