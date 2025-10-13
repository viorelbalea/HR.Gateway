using System.Net.Http;
using HR.Gateway.Application.Abstractions.MFiles;
using Microsoft.Extensions.Logging;

namespace HR.Gateway.Infrastructure.MFiles.Clients;

public sealed class MFilesClient : IMFilesClient
{
    private readonly HttpClient _http;
    private readonly IMFilesTokenProvider _tokens;
    private readonly ILogger<MFilesClient> _log;

    public MFilesClient(HttpClient http, IMFilesTokenProvider tokens, ILogger<MFilesClient> log)
    {
        _http = http;
        _tokens = tokens;
        _log = log;
    }

    public async Task<string> PingAsync(CancellationToken ct = default)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "structure/objecttypes");
        req.Headers.TryAddWithoutValidation("X-Authentication", await _tokens.GetTokenAsync(ct));

        var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if ((int)resp.StatusCode is 401 or 403)
        {
            _log.LogWarning("MFWS returned {Status}. Invalidating token and retrying once.", (int)resp.StatusCode);
            _tokens.Invalidate();

            var retry = new HttpRequestMessage(HttpMethod.Get, "structure/objecttypes");
            retry.Headers.TryAddWithoutValidation("X-Authentication", await _tokens.GetTokenAsync(ct));
            resp = await _http.SendAsync(retry, ct);
            body = await resp.Content.ReadAsStringAsync(ct);
        }

        resp.EnsureSuccessStatusCode();
        return body; // va fi JSON cu object types
    }

}