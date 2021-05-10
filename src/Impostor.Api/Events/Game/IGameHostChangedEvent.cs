using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGameHostChangedEvent : IGameEvent
    {
        IClientPlayer PreviousHost { get; }

        IClientPlayer? NewHost { get; }
    }
}
