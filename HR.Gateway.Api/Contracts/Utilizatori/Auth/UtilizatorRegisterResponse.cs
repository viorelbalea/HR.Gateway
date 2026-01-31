namespace HR.Gateway.Api.Contracts.Utilizatori.Auth;

public class UtilizatorRegisterResponse
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public string? Role { get; init; }
}