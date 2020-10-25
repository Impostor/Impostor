using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public class PlayerSpawnedEvent : IGameEvent
    {
        public PlayerSpawnedEvent(IGame game, IInnerPlayerControl playerControl)
        {
            Game = game;
            PlayerControl = playerControl;
        }

        public IGame Game { get; }

        public IInnerPlayerControl PlayerControl { get; }
    }
}