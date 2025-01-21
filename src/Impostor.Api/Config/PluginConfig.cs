namespace Impostor.Api.Config;

public class PluginConfig
{
    public const string Section = "Plugin";

    public string[] PluginPaths { get; set; } = ["plugins"];

    public string[] LibraryPaths { get; set; } = ["libraries"];
}
