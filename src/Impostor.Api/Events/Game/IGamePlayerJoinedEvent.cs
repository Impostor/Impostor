using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGamePlayerJoinedEvent : IGameEvent
    {
        IClientPlayer Player { get; }
    }
}
