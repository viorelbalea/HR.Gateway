namespace HR.Gateway.Application.Abstractions.Security;

public interface ITokenFactory
{
    Task<string> CreateJwtAsync(Guid userId, string? email, string? userName, IEnumerable<string> roles, CancellationToken ct = default);
}