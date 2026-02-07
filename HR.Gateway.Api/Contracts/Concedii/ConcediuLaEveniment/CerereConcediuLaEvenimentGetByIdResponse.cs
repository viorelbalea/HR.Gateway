namespace HR.Gateway.Api.Contracts.Concedii.ConcediuLaEveniment;

public sealed class CerereConcediuLaEvenimentGetByIdResponse
{
    public int Id { get; init; }
    public DateTime? DataCerere { get; init; }
    public DateTime? DataInceput { get; init; }
    public int? NumarZile { get; init; }
    public string TipEveniment { get; init; } = string.Empty;
    public int TipEvenimentId { get; init; }
    public string Inlocuitor { get; init; } = string.Empty;
    public string NumarInregistrare { get; init; } = string.Empty;
    public DateTime? DataInregistrare { get; init; }
    public string State { get; init; } = string.Empty;
}
