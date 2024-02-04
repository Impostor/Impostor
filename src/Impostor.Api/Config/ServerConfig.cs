using Impostor.Api.Utils;

namespace Impostor.Api.Config;

public class ServerConfig
{
    public const string Section = "Server";
    private string? _resolvedListenIp;

    private string? _resolvedPublicIp;

    public string PublicIp { get; set; } = "127.0.0.1";

    public ushort PublicPort { get; set; } = 22023;

    public string ListenIp { get; set; } = "127.0.0.1";

    public ushort ListenPort { get; set; } = 22023;

    public bool UseUDP { get; set; } = true;

    public bool UseOnline { get; set; } = true;

    public bool UseDtl { get; set; } = false;

    public string ServerCertification { get; set; } = string.Empty;

    public string ResolvePublicIp()
    {
        return _resolvedPublicIp ??= IpUtils.ResolveIp(PublicIp);
    }

    public string ResolveListenIp()
    {
        return _resolvedListenIp ??= IpUtils.ResolveIp(ListenIp);
    }
}
