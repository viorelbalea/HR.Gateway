using HR.Gateway.Application.Models.CerereConcediu;

namespace HR.Gateway.Application.Abstractions.CerereConcediu;

public interface IInlocuitoriProvider
{
    Task<IReadOnlyList<Inlocuitor>> GetInlocuitoriPentruEmailAsync(
        string email,
        CancellationToken ct = default);
}

