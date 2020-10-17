using Impostor.Server.Games;

namespace Impostor.Server.Events
{
    /// <summary>
    ///     Called whenever a new <see cref="IGame"/> is destroyed.
    /// </summary>
    public sealed class GameDestroyedEvent : IGameEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GameDestroyedEvent"/> class.
        /// </summary>
        /// <param name="game">Instance of the game.</param>
        public GameDestroyedEvent(IGame game)
        {
            Game = game;
        }

        /// <inheritdoc/>
        public IGame Game { get; }
    }
}