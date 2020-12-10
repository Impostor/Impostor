using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events
{
    public interface IGamePlayerJoinedEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer"/> which triggered the event.
        ///     The <see cref="IInnerPlayerControl"/> Character of this object is null since the Character object didn't spawn yet.
        /// </summary>
        IClientPlayer Player { get; }
    }
}
