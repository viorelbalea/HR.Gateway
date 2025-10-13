namespace HR.Gateway.Infrastructure.MFiles;

public sealed class MFilesOptions
{
    public string ActiveEnvironment { get; set; } = "Dev";
    public Dictionary<string, MFilesEnvironment> Environments { get; set; } = new();
}