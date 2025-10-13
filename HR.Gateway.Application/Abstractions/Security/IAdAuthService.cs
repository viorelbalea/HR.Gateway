namespace HR.Gateway.Application.Abstractions.Security;

public interface IAdAuthService
{
    Task<bool> ValidateAsync(string username, string password);
}