namespace HR.Gateway.Infrastructure.MFiles.Tokens;

public sealed class MFilesCredentials
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string VaultGuid { get; set; } = "";
}