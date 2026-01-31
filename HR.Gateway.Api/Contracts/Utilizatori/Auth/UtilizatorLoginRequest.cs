namespace HR.Gateway.Api.Contracts.Utilizatori.Auth;

public class UtilizatorLoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}