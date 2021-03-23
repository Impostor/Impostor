using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called just before a new <see cref="IGame"/> is created.
    /// </summary>
    public interface IBeforeGameCreatedEvent : IEvent
    {
        /// <summary>
        ///     Gets the <see cref="Games.GameCode"/> set by this event.
        /// </summary>
        GameCode? GameCode { get; }

        /// <summary>
        ///     Tries to set the game code to be used. This doesn't garantee the code will be used, if another lobby has been created with the same code before this event returns for exemple.
        /// </summary>
        /// <param name="gameCode">The code to try to set.</param>
        /// <exception cref="System.ArgumentException">If the argument is not 6 characters long or does not contains only letters.</exception>
        /// <returns>True if the code has been set. False if the code already exists.</returns>
        bool TryToSetCode(GameCode gameCode);
    }
}
