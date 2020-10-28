using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerMurderEvent : IPlayerMurderEvent
    {
        public PlayerMurderEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, IInnerPlayerControl victim)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            Victim = victim;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public IInnerPlayerControl Victim { get; }
    }
}
