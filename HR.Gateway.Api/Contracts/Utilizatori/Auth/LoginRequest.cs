namespace HR.Gateway.Api.Contracts.Utilizatori.Auth;

public class LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}