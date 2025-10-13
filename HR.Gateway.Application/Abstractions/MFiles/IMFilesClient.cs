namespace HR.Gateway.Application.Abstractions.MFiles;

public interface IMFilesClient
{
    Task<string> PingAsync(CancellationToken ct = default);
}