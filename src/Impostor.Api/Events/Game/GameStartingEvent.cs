using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called when the game is going to start.
    ///     When this is called, not all players are initialized properly yet.
    ///     If you want to get correct player states, use <see cref="GameStartedEvent"/>.
    /// </summary>
    public class GameStartingEvent : IGameEvent
    {
        public GameStartingEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}