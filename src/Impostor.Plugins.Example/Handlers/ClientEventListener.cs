using Impostor.Api.Events;
using Impostor.Api.Events.Client;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example.Handlers
{
    public class ClientEventListener : IEventListener
    {
        private readonly ILogger<ClientEventListener> _logger;

        public ClientEventListener(ILogger<ClientEventListener> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnClientConnected(IClientConnectedEvent e)
        {
            _logger.LogInformation("Client {name} > connected (language: {language}, chat mode: {chatMode})", e.Client.Name, e.Client.Language, e.Client.ChatMode);
        }
    }
}
