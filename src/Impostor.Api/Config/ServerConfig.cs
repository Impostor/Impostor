using Serilog.Events;

namespace Impostor.Api.Config;

public class ServerConfig
{
    public const string Section = "Server";

    public ListenerConfig[] Listeners { get; set; } = [];

    public bool EnableCommands { get; set; }
    public bool EnableNextApi { get; set; }
    public bool EnableCustom { get; set; }
    
    public bool WriteConsole { get; set; } = true;
    public bool WriteFile { get; set; } = true;
    public string LogFilePath { get; set; } = "LogOut.log";
    public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

    public string? RootPath { get; set; }
    public string? Env { get; set; }
}

#pragma warning disable SA1402
public class ListenerConfig
{
    public string? PublicIp { get; set; }
    public ushort? PublicPort { get; set; }
    public string ListenIp { get; set; } = "0.0.0.0";
    public ushort ListenPort { get; set; } = 22023;
    public bool IsDtl { get; set; } = false;
    public string PrivateKeyPath { get; set; } = string.Empty;
    public string CertificatePath { get; set; } = string.Empty;

    public bool HasAuth { get; set; } = false;
    // No Set Use First PublicIp or ListenIp

    // No Set Use ListenPort

    // PrivateKeyPath must be set

    // Dtl Use

    // Dtl Use

    // Port is +2, if is dtl no use, auth use dtl so private key must be set
}
