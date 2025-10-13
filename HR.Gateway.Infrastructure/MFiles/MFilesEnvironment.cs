namespace HR.Gateway.Infrastructure.MFiles;

public sealed class MFilesEnvironment
{
    public string BaseUrl { get; set; } = "";
    public MFilesCredentials Credentials { get; set; } = new();
    public MFilesMetadataConfig Metadata { get; set; } = new();
}