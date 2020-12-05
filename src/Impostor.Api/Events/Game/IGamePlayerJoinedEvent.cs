using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGamePlayerJoinedEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer"/> which triggered the event.
        /// </summary>
        IClientPlayer Player { get; }
    }
}
