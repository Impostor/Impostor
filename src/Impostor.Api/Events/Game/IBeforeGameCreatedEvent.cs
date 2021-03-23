using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called just before a new <see cref="IGame"/> is created.
    /// </summary>
    public interface IBeforeGameCreatedEvent : IEvent
    {
        /// <summary>
        ///     Gets or sets the desired <see cref="Games.GameCode"/>.
        /// </summary>
        /// <exception cref="ImpostorException">If the GameCode is invalid or already used in another game.</exception>
        GameCode? GameCode { get; set; }
    }
}
