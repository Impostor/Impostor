using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     The game is started here and players have been initialized.
    /// </summary>
    public class GameStartedEvent : IGameEvent
    {
        public GameStartedEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}