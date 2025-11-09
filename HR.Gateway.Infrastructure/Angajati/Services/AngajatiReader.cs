using HR.Gateway.Application.Abstractions.Angajati;
using HR.Gateway.Application.Models.Angajati;
using HR.Gateway.Application.Models.Concedii;
using HR.Gateway.Application.Models.ConcediiPerAngajat;
using HR.Gateway.Infrastructure.Angajati.Client;

namespace HR.Gateway.Infrastructure.Angajati.Services;

internal sealed class AngajatiReader : IAngajatiReader
{
    private readonly IVemAngajatiClient _client;
    public AngajatiReader(IVemAngajatiClient client) => _client = client;

    public async Task<AngajatView?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return null;

        var r = await _client.GetByEmailAsync(normalized, ct);
        var a = r?.Angajat;
        if (a is null) return null;

        return new AngajatView
        {
            Email       = a.Email ?? normalized,
            NumeComplet = a.NumeComplet ?? $"{a.Prenume} {a.Nume}".Trim(),
            Prenume     = a.Prenume,
            Nume        = a.Nume,
            Departament = a.Departament,
            Functie     = a.Functie
        };
    }
    
    public async Task<SituatieConcediiAngajat?> GetOverviewAsync(string email, CancellationToken ct)
    {
        var r = await _client.GetByEmailAsync(email, ct);
        if (r is null) return null;
        if (r.Angajat is null) return null;

        var a = r.Angajat;
        var concedii = r.Concedii ?? new List<HR.Gateway.Infrastructure.Angajati.Client.Dtos.Overview.VemConcediu>();

        var peAniDetaliat = concedii
            .OrderByDescending(c => c.An)
            .Select(c => new ConcediuPeAnDetaliat
            {
                An        = c.An,
                AnId      = c.AnId,
                Alocate   = c.NumarZileAlocate,
                Consumate = c.NumarZileConsumate,
                Ramase    = c.NumarZileRamase
            })
            .ToList();

        var profil = new AngajatView
        {
            Email       = a.Email ?? email,
            NumeComplet = a.NumeComplet ?? $"{a.Prenume} {a.Nume}".Trim(),
            Prenume     = a.Prenume,
            Nume        = a.Nume,
            Departament = a.Departament,
            Functie     = a.Functie
        };

        var totalDisponibile = a.ZileConcediuRamase ?? peAniDetaliat.Sum(x => x.Ramase);

        return new SituatieConcediiAngajat
        {
            Profil           = profil,
            TotalZileDisponibile = totalDisponibile,
            PeAniDetaliat    = peAniDetaliat
        };
    }
}