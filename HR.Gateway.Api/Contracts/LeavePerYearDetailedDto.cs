namespace HR.Gateway.Api.Contracts;

public sealed class LeavePerYearDetailedDto
{
    public int An { get; init; }
    public int AnId { get; init; }
    public int NumarZileAlocate { get; init; }
    public int NumarZileConsumate { get; init; }
    public int NumarZileRamase { get; init; }
}