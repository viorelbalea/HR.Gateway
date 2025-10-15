namespace HR.Gateway.Api.Contracts;

public sealed class UserOverviewDto
{
    public required UserProfileDto Profile { get; init; }
    public required LeaveOverviewDto Leave { get; init; }
}