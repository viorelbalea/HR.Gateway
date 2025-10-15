namespace HR.Gateway.Application.Employees.Models;

public sealed class EmployeeOverview
{
    public required EmployeeProfile Profile { get; init; }
    public required int TotalAvailable { get; init; }
    public required IReadOnlyList<LeavePerYear> PerYear { get; init; }
    public IReadOnlyList<LeavePerYearDetailed>? PerYearDetailed { get; init; }
}