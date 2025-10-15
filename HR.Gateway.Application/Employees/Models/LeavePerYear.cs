namespace HR.Gateway.Application.Employees.Models;

public sealed class LeavePerYear
{
    public required int Year { get; init; }
    public required int Available { get; init; }
}