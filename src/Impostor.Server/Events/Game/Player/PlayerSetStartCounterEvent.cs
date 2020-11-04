using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Game.Player
{
    public class PlayerSetStartCounterEvent : IPlayerSetStartCounterEvent
    {
        public PlayerSetStartCounterEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, byte value)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            Value = value;
        }

        public byte Value { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public IGame Game { get; }
    }
}
