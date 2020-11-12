using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin(
        package: "gg.impostor.example",
        name: "Example",
        author: "AeonLucid",
        version: "1.0.0")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;

        public ExamplePlugin(ILogger<ExamplePlugin> logger)
        {
            _logger = logger;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            return default;
        }
    }
}
