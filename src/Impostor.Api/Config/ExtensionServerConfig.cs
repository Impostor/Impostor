namespace Impostor.Api.Config;

public class ExtensionServerConfig
{
    public const string Section = "ExtensionServer";

    public bool Enable { get; set; } = false;

    public string ListenIp { get; set; } = "0.0.0.0";

    public ushort ListenPort { get; set; } = 22025;

    public bool EnabledSpa { get; set; } = false;

    public string SpaDirectory { get; set; } = "/Web";

    public bool EnabledNextApi { get; set; } = false;
    
    public string SignalRToken { get; set; } = "NextImpostor";

    public bool EnabledSignalRWeb { get; set; } = false;
    
    public string MatchmakerToken { get; set; } = "NextImpostor";

    public bool EnabledHttpMatchmaker { get; set; } = false;
    
    public bool EnabledWebSocketMatchmaker { get; set; } = false;
}
