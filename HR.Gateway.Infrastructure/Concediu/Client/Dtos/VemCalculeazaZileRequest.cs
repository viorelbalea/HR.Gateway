namespace HR.Gateway.Infrastructure.Concediu.Client.Dtos;

internal sealed class VemCalculeazaZileRequest
{
    public required string   Email       { get; init; }
    public required DateTime DataInceput { get; init; }
    public required DateTime DataSfarsit { get; init; }
}
