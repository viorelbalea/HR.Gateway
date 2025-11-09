namespace HR.Gateway.Infrastructure.Concedii.Client.Dtos;

public sealed class VemCalculeazaZileResponse
{
    public bool FoundSuccess        { get; init; }
    public int  NumarZileCalculate  { get; init; }
    public List<VemHolidayDto>? Holidays { get; init; }
}