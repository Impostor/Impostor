using Impostor.Api.Config;

namespace Impostor.Api.Net.Manager;

public interface INetListenerManager
{
    ListenerConfig? GetAvailableListener();
}

