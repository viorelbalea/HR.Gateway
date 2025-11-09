namespace HR.Gateway.Api.Contracts.Concedii.Common;

public sealed class CalculeazaZileRequest
{
    public required string   Email       { get; init; }
    public required DateTime DataInceput { get; init; }
    public required DateTime DataSfarsit { get; init; }
}