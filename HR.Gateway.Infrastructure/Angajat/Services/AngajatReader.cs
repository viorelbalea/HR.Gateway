using HR.Gateway.Application.Abstractions.Angajati;
using HR.Gateway.Application.Models.Angajati;
using HR.Gateway.Application.Models.Concedii;
using HR.Gateway.Application.Models.ConcediiPerAngajat;
using HR.Gateway.Infrastructure.Angajat.Client;

namespace HR.Gateway.Infrastructure.Angajat.Services;

internal sealed class AngajatReader : IAngajatReader
{
    private readonly IVemAngajatClient _client;
    public AngajatReader(IVemAngajatClient client) => _client = client;

    public async Task<AngajatGetByEmailResponse?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return null;

        var r = await _client.GetByEmailAsync(normalized, ct);
        var a = r?.Angajat;
        if (a is null) return null;

        return new AngajatGetByEmailResponse
        {
            Email       = a.Email ?? normalized,
            NumeComplet = a.NumeComplet ?? $"{a.Prenume} {a.Nume}".Trim(),
            Prenume     = a.Prenume,
            Nume        = a.Nume,
            Departament = a.Departament,
            Functie     = a.Functie
        };
    }
    
    public async Task<AngajatGetOverviewResponse?> GetOverviewAsync(string email, CancellationToken ct)
    {
        var r = await _client.GetByEmailAsync(email, ct);
        if (r is null) return null;
        if (r.Angajat is null) return null;

        var a = r.Angajat;
        var concedii = r.Concedii ?? new List<HR.Gateway.Infrastructure.Angajat.Client.Dtos.Overview.VemConcediu>();

        var peAniDetaliat = concedii
            .OrderByDescending(c => c.An)
            .Select(c => new ConcediuYearDetailItem
            {
                An        = c.An,
                AnId      = c.AnId,
                Alocate   = c.NumarZileAlocate,
                Consumate = c.NumarZileConsumate,
                Ramase    = c.NumarZileRamase
            })
            .ToList();

        var profil = new AngajatGetByEmailResponse
        {
            Email       = a.Email ?? email,
            NumeComplet = a.NumeComplet ?? $"{a.Prenume} {a.Nume}".Trim(),
            Prenume     = a.Prenume,
            Nume        = a.Nume,
            Departament = a.Departament,
            Functie     = a.Functie
        };

        var totalDisponibile = a.ZileConcediuRamase ?? peAniDetaliat.Sum(x => x.Ramase);

        return new AngajatGetOverviewResponse
        {
            Profil           = profil,
            TotalZileDisponibile = totalDisponibile,
            PeAniDetaliat    = peAniDetaliat
        };
    }
}
