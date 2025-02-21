namespace Impostor.Api.Config;

public class ListenerConfig
{
    public string PublicIp { get; set; } = "127.0.0.1";
    public ushort PublicPort { get; set; } = 22023;
    public string ListenIp { get; set; } = "0.0.0.0";
    public ushort ListenPort { get; set; } = 22023;
    public bool IsDtl { get; set; } = false;

    public string PrivateKeyPath { get; set; } = string.Empty;
    public string CertificatePath { get; set; } = string.Empty;

    public bool HasAuth { get; set; } = false;
}
