using Impostor.Api.Net;

namespace Impostor.Api.Events
{
    public interface IGamePlayerLeftEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer"/> which triggered the event.
        /// </summary>
        IClientPlayer Player { get; }
        
        /// <summary>
        ///     Gets the the info if the player was banned or just left
        /// </summary>
        bool IsBan { get; }
    }
}
