using System.Net.Http;
using System.Net.Http.Json;
using HR.Gateway.Application.Abstractions.CerereConcediu;
using HR.Gateway.Application.Models.CerereConcediu;
using HR.Gateway.Application.Models.Concedii;
using Microsoft.Extensions.Logging;

namespace HR.Gateway.Infrastructure.Concediu.Services;

// IMPORTANT: trebuie sa fie public ?i concret.
public sealed class InlocuitoriProvider : IInlocuitoriProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<InlocuitoriProvider> _log;

    // IMPORTANT: exact acest constructor, public.
    public InlocuitoriProvider(HttpClient http, ILogger<InlocuitoriProvider> log)
    {
        _http = http;
        _log = log;
    }

    // DTO-urile pentru raspunsul din VEM
    private sealed class VemCerereConcediuOdihnaGetReplacementsResponse
    {
        public bool FoundSuccess { get; init; }
        public string? Message { get; init; }
        public List<VemInlocuitorItem> Inlocuitori { get; init; } = new();
    }

    private sealed class VemInlocuitorItem
    {
        public string Email { get; init; } = "";
        public string NumeComplet { get; init; } = "";
    }

    public async Task<IReadOnlyList<Inlocuitor>> GetInlocuitoriPentruEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var payload = new { Email = email };

        // ? vezi exact ce URL apelezi
        var relativeUrl = "vault/extensionmethod/GetPotentialReplacementsExtensionMethod";
        _log.LogInformation("Inlocuitori: POST {Base}{Url} pentru {Email}",
            _http.BaseAddress, relativeUrl, email);

        using var resp = await _http.PostAsJsonAsync(relativeUrl, payload, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            _log.LogError("Inlocuitori: {Status} pentru {Base}{Url}. Body: {Body}",
                (int)resp.StatusCode, _http.BaseAddress, relativeUrl, body);

            // daca e 404, �ntoarcem lista goala (sa nu mai pice tot API-ul).
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Array.Empty<Inlocuitor>();

            resp.EnsureSuccessStatusCode(); // re-throw pentru alte coduri
        }

        var vem = await resp.Content.ReadFromJsonAsync<VemCerereConcediuOdihnaGetReplacementsResponse>(cancellationToken: ct)
                  ?? new VemCerereConcediuOdihnaGetReplacementsResponse();

        if (!vem.FoundSuccess || vem.Inlocuitori.Count == 0)
            return Array.Empty<Inlocuitor>();

        return vem.Inlocuitori
            .Select(x => new Inlocuitor
            {
                NumeComplet = x.NumeComplet,
                Email       = x.Email
            })
            .ToArray();
    }

}




