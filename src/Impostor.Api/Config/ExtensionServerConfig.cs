namespace Impostor.Api.Config;

public class ExtensionServerConfig
{
    public const string Section = "ExtensionServer";

    public bool Enable { get; set; } = true;

    public string ListenIp { get; set; } = "127.0.0.1";

    public ushort ListenPort { get; set; } = 22025;

    public bool EnabledSpa { get; set; } = false;

    public string SpaDirectory { get; set; } = "/Web";

    public bool UseAuth { get; set; } = true;
    public string Token { get; set; } = "NextImpostor";

    public bool EnabledNextApi { get; set; } = false;

    public bool EnabledSignalR { get; set; } = false;

    public bool EnabledHttpApi { get; set; } = true;

    public bool EnabledWebSocket{ get; set; } = false;

    public long WebSocketInterval { get; set; } = 120;

    public long WebSocketTimeout { get; set; } = 3000;
}
