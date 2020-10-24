using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects;

namespace Impostor.Api.Events.Net
{
    public class PlayerSpawnedEvent : IGameEvent
    {
        public PlayerSpawnedEvent(IGame game, InnerPlayerControl playerControl)
        {
            Game = game;
            PlayerControl = playerControl;
        }

        public IGame Game { get; }

        public InnerPlayerControl PlayerControl { get; }
    }
}