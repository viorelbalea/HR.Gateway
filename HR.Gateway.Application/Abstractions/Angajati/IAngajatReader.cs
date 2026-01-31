using HR.Gateway.Application.Models.Angajati;
using HR.Gateway.Application.Models.ConcediiPerAngajat;

namespace HR.Gateway.Application.Abstractions.Angajati;

public interface IAngajatReader
{
    /// <summary>
    /// Returnează datele de bază ale angajatului identificat prin email (sau null dacă nu există).
    /// </summary>
    Task<AngajatGetByEmailResponse?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Returnează situația concediilor pentru angajatul identificat prin email (sau null dacă angajatul nu există).
    /// </summary>
    Task<AngajatGetOverviewResponse?> GetOverviewAsync(string email, CancellationToken ct = default);
}
