using Impostor.Api.Plugins;

namespace Impostor.Plugins.Debugger
{
    [ImpostorPlugin(
        package: "gg.impostor.debugger",
        name: "Debugger",
        author: "Gerard",
        version: "1.0.0",
        dependencies: new string[] {},
        softDependencies: new string[] {},
        loadBefore: new string[] {})]
    public class DebugPlugin : PluginBase
    {
    }
}
