namespace HR.Gateway.Api.Contracts;

public sealed class UserProfileDto
{
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public string? FirstName { get; init; }  
    public string? LastName  { get; init; }  
    public string? Department { get; init; }
    public string? Position { get; init; }
}