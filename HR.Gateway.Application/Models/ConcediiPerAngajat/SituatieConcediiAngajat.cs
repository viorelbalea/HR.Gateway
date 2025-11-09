using HR.Gateway.Application.Models.Angajati;
using HR.Gateway.Application.Models.Concedii;

namespace HR.Gateway.Application.Models.ConcediiPerAngajat;

/// <summary>
/// Situația agregată a concediilor pentru un angajat.
/// </summary>
public sealed class SituatieConcediiAngajat
{
    /// <summary>Profilul de bază al angajatului (nume, email etc.).</summary>
    public required AngajatView Profil { get; init; }

    /// <summary>Totalul de zile disponibile la momentul generării situației.</summary>
    public required int TotalZileDisponibile { get; init; }

    /// <summary>Detalierea pe ani. Întotdeauna o listă (poate fi goală), niciodată null.</summary>
    public IReadOnlyList<ConcediuPeAnDetaliat> PeAniDetaliat { get; init; } = Array.Empty<ConcediuPeAnDetaliat>();
}