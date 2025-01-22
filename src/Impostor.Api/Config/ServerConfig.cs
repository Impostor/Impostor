using System.Net;

namespace Impostor.Api.Config;

using Serilog.Events;

public class ServerConfig
{
    public const string Section = "Server";

    public ListenerConfig[] Listeners { get; set; } = [];

    public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

    public string? Env { get; set; }
}

#pragma warning disable SA1402
public class ListenerConfig
{
    // No Set Use First PublicIp or ListenIp
    public string? PublicIp { get; set; }

    // No Set Use ListenPort
    public ushort? PublicPort { get; set; }

    public string ListenIp { get; set; } = "0.0.0.0";

    public ushort ListenPort { get; set; } = 22023;

    // PrivateKeyPath must be set
    public bool IsDtl { get; set; } = false;

    // Dtl Use
    public string PrivateKeyPath { get; set; } = string.Empty;

    // Dtl Use
    public string CertificatePath { get; set; } = string.Empty;

    // Port is +2, if is dtl no use, auth use dtl so private key must be set
    public bool HasAuth { get; set; } = false;
}
