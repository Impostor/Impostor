namespace Impostor.Api.Config;

public class ExtensionServerConfig
{
    public const string Section = "ExtensionServer";

    public bool Enabled { get; set; }

    public string ListenIp { get; set; } = "127.0.0.1";

    public ushort ListenPort { get; set; } = 22025;

    public bool EnabledSpa { get; set; }

    public string SpaDirectory { get; set; } = "/Web";

    public bool UseAuth { get; set; }
    public string Token { get; set; } = "NextImpostor";

    public bool EnabledNextApi { get; set; }

    public bool EnabledSignalR { get; set; }

    public bool EnabledHttpApi { get; set; }

    public bool EnabledWebSocket{ get; set; }

    public long WebSocketInterval { get; set; } = 120;

    public long WebSocketTimeout { get; set; } = 3000;
}
