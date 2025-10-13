namespace HR.Gateway.Application.Abstractions.MFiles;

public interface IMFilesTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken ct = default);

    void Invalidate();
}