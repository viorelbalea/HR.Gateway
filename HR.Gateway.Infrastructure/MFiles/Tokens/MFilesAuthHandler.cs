// HR.Gateway.Infrastructure/MFiles/Tokens/MFilesAuthHandler.cs

using HR.Gateway.Application.Abstractions.MFiles;

namespace HR.Gateway.Infrastructure.MFiles.Tokens;

public sealed class MFilesAuthHandler : DelegatingHandler
{
    private readonly IMFilesTokenProvider _tokens;
    public MFilesAuthHandler(IMFilesTokenProvider tokens) => _tokens = tokens;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _tokens.GetTokenAsync(ct);

        request.Headers.Remove("X-Authentication");
        request.Headers.Add("X-Authentication", token);

        var response = await base.SendAsync(request, ct);

        // Dacă sesiunea M-Files a expirat (403 + ErrorCode 27), invalidăm token-ul și reîncercăm o singură dată
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            if (content.Contains("\"ErrorCode\":\"27\""))
            {
                _tokens.Invalidate();

                // Clonăm request-ul pentru retry (altfel body-ul e deja consumat)
                var retryRequest = await CloneRequestAsync(request, ct);

                // Retry cu token fresh
                token = await _tokens.GetTokenAsync(ct);
                retryRequest.Headers.Remove("X-Authentication");
                retryRequest.Headers.Add("X-Authentication", token);

                response.Dispose();
                response = await base.SendAsync(retryRequest, ct);
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original, CancellationToken ct)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri)
        {
            Version = original.Version
        };

        if (original.Content != null)
        {
            var contentBytes = await original.Content.ReadAsByteArrayAsync(ct);
            clone.Content = new ByteArrayContent(contentBytes);

            // Copiază headers de la Content
            foreach (var header in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copiază headers de la Request (fără X-Authentication care se va seta mai târziu)
        foreach (var header in original.Headers)
        {
            if (header.Key != "X-Authentication")
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}