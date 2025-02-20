using Serilog.Events;

namespace Impostor.Api.Config;

public class ServerConfig
{
    public const string Section = "Server";

    public ListenerConfig[] Listeners { get; set; } = [];

    public bool EnableCommands { get; set; }
    public string CommandPrefix { get; set; } = "/";
    public bool EnableNextApi { get; set; }
    
    public bool WriteConsole { get; set; } = true;
    public bool WriteFile { get; set; } = true;
    public string LogFilePath { get; set; } = "LogOut.log";
    public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

    public string? RootPath { get; set; }
    public string? Env { get; set; }
}
