namespace HR.Gateway.Infrastructure.MFiles.Tokens;

public sealed class MFilesEnvironment
{
    public string BaseUrl { get; set; } = "";
    public MFilesCredentials Credentials { get; set; } = new();
    public MFilesMetadataConfig Metadata { get; set; } = new();
}