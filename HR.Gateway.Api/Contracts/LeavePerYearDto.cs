namespace HR.Gateway.Api.Contracts;

public sealed class LeavePerYearDto
{
    public required int Year { get; init; }
    public required int Available { get; init; }
}