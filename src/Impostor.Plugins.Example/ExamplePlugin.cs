using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.Example.Handlers;
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
        private readonly IEventManager _eventManager;
        private IDisposable[] _cancel;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IEventManager eventManager)
        {
            _logger = logger;
            _eventManager = eventManager;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");

            _cancel = new[]
            {
                _eventManager.RegisterListener(new GameEventListener()),
                _eventManager.RegisterListener(new PlayerEventListener()),
                _eventManager.RegisterListener(new MeetingEventListener())
            };

            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");

            foreach (var c in _cancel)
            {
                c.Dispose();
            }

            return default;
        }
    }
}
