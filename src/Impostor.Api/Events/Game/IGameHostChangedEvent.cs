using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGameHostChangedEvent : IGameEvent
    {
        IClientPlayer Host { get; }

        IClientPlayer OldHost { get; }
    }
}
