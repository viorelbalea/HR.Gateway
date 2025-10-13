using System.Net.Http;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using HR.Gateway.Application.Abstractions.MFiles;
using HR.Gateway.Infrastructure.MFiles;

namespace HR.Gateway.Infrastructure.MFiles.Tokens;

public sealed class MFilesTokenProvider : IMFilesTokenProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<MFilesTokenProvider> _log;
    private readonly MFilesEnvironment _env;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private string? _token;
    private DateTimeOffset _obtainedAt;

    public MFilesTokenProvider(
        HttpClient http,
        IOptions<MFilesOptions> opts,
        ILogger<MFilesTokenProvider> log)
    {
        _http = http;
        _log = log;

        var settings = opts.Value;
        var envName  = settings.ActiveEnvironment;
        _env = settings.Environments[envName];
    }

    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_token))
            return _token!;

        await _gate.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrEmpty(_token))
                return _token!; // double-check

            var payload = new
            {
                Username  = _env.Credentials.Username,
                Password  = _env.Credentials.Password,
                VaultGuid = _env.Credentials.VaultGuid
            };

            var opts = new JsonSerializerOptions { PropertyNamingPolicy = null }; // păstrează PascalCase
            var json = JsonSerializer.Serialize(payload, opts);

            var resp = await _http.PostAsync(
                "server/authenticationtokens.aspx",
                new StringContent(json, Encoding.UTF8, "application/json"),
                ct);

            var body = await resp.Content.ReadAsStringAsync(ct);
            resp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(body);
            _token = doc.RootElement.GetProperty("Value").GetString()
                     ?? throw new InvalidOperationException("MFWS token missing 'Value'.");

            _obtainedAt = DateTimeOffset.UtcNow;
            _log.LogInformation("MFWS token obtained at {t}.", _obtainedAt);

            return _token!;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Invalidate() => _token = null;
}
