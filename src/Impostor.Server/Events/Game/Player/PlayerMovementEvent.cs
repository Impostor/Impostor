using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    // TODO: Finish and use event, needs to be pooled
    public class PlayerMovementEvent : IPlayerEvent
    {
        public PlayerMovementEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }
    }
}
