namespace HR.Gateway.Api.Contracts;

public sealed class LeaveOverviewDto
{
    public required int TotalAvailable { get; init; }
    public required IReadOnlyList<LeavePerYearDto> PerYear { get; init; }
    public IReadOnlyList<LeavePerYearDetailedDto>? PerYearDetailed { get; init; }
}