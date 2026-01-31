namespace HR.Gateway.Api.Contracts.Concedii.ConcediuFaraPlata;

public sealed class CreareCerereConcediuFaraPlataResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public int CerereConcediuFaraPlataId { get; init; }
}