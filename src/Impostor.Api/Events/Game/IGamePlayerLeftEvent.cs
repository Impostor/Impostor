using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGamePlayerLeftEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer"/> which triggered the event.
        ///     The <see cref="IInnerPlayerControl"/> Character of this object is null since the Character object was already disposed.
        /// </summary>
        IClientPlayer Player { get; }

        /// <summary>
        ///     Gets a value indicating whether the player was banned or just left.
        /// </summary>
        bool IsBan { get; }
    }
}
