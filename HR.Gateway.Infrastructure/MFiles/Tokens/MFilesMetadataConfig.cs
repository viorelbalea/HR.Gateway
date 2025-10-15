namespace HR.Gateway.Infrastructure.MFiles.Tokens;

public sealed class MFilesMetadataConfig
{
    public Dictionary<string, KeySpec> ObjectTypes { get; set; } = new();
    public Dictionary<string, KeySpec> Classes     { get; set; } = new();
    public Dictionary<string, KeySpec> Properties  { get; set; } = new();
    public Dictionary<string, KeySpec> ValueLists  { get; set; } = new();
    public Dictionary<string, KeySpec> Workflows   { get; set; } = new();
    public Dictionary<string, KeySpec> States      { get; set; } = new();
}