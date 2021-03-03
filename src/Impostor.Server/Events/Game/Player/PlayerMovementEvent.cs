using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server.Events.Player
{
    public class PlayerMovementEvent : IPlayerMovementEvent
    {
#pragma warning disable 8766
        public IGame? Game { get; private set; }

        public IClientPlayer? ClientPlayer { get; private set; }

        public IInnerPlayerControl? PlayerControl { get; private set; }
#pragma warning restore 8766

        public void Reset(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
        }

        public void Reset()
        {
            Game = null;
            ClientPlayer = null;
            PlayerControl = null;
        }

        public class PlayerMovementEventObjectPolicy : IPooledObjectPolicy<PlayerMovementEvent>
        {
            public PlayerMovementEvent Create()
            {
                return new PlayerMovementEvent();
            }

            public bool Return(PlayerMovementEvent obj)
            {
                obj.Reset();
                return true;
            }
        }
    }
}
