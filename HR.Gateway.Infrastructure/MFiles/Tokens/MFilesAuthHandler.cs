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

        return await base.SendAsync(request, ct);
    }
}