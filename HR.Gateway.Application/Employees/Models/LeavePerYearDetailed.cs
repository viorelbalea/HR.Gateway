namespace HR.Gateway.Application.Employees.Models;

public sealed class LeavePerYearDetailed
{
    public required int Year { get; init; }
    public required int YearId { get; init; }
    public required int Allocated { get; init; }
    public required int Used { get; init; }
    public required int Available { get; init; }
}