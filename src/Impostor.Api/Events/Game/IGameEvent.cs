using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    public interface IGameEvent : IEvent
    {
        /// <summary>
        ///     Gets the <see cref="IGame" /> this event belongs to.
        /// </summary>
        IGame Game { get; }
    }
}
