using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Net
{
    public class PlayerDestroyedEvent : IGameEvent
    {
        public PlayerDestroyedEvent(IGame game, IInnerPlayerControl playerControl)
        {
            Game = game;
            PlayerControl = playerControl;
        }

        public IGame Game { get; }

        public IInnerPlayerControl PlayerControl { get; }
    }
}