namespace HR.Gateway.Api.Contracts.Utilizatori.Auth;

public class RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? Role { get; init; }
}