using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called whenever a new <see cref="IGame"/> is created.
    /// </summary>
    public sealed class GameCreatedEvent : IGameEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GameCreatedEvent"/> class.
        /// </summary>
        /// <param name="game">Instance of the game.</param>
        public GameCreatedEvent(IGame game)
        {
            Game = game;
        }

        /// <inheritdoc/>
        public IGame Game { get; }
    }
}