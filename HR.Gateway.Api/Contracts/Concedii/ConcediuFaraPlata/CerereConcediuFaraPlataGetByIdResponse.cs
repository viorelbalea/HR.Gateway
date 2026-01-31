namespace HR.Gateway.Api.Contracts.Concedii.ConcediuFaraPlata;

public sealed class CerereConcediuFaraPlataGetByIdResponse
{
    public int Id { get; init; }
    public DateTime? DataCerere { get; init; }

    public DateTime? DataInceput { get; init; }
    public DateTime? DataSfarsit { get; init; }

    public int? NumarZile { get; init; }
    public string Stare { get; init; } = string.Empty;

    public string Inlocuitor { get; init; } = string.Empty;

    public string NumarInregistrare { get; init; } = string.Empty;
    public DateTime? DataInregistrare { get; init; }

    public string? Motiv { get; init; }
}