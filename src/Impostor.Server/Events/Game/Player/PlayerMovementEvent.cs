using System;
using System.Collections.Concurrent;

using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Hazel;

namespace Impostor.Server.Events.Player
{
    // TODO: Finish and use event, needs to be pooled
    public class PlayerMovementEvent : IPlayerMovementEvent , IRecyclable , IDisposable
    {
        private static readonly ConcurrentBag<PlayerMovementEvent> items = new ConcurrentBag<PlayerMovementEvent>();
        private static int counter = 0;
        private static int MAX = 25;

        public PlayerMovementEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
        }

        public IGame Game { get; private set; }

        public IClientPlayer ClientPlayer { get; private set; }

        public IInnerPlayerControl PlayerControl { get; private set; }

        public static PlayerMovementEvent Get(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl)
        {
            PlayerMovementEvent item;
            if (items.TryTake(out item))
            {
                counter--;
                item.Clear(game, clientPlayer, playerControl);
                return item;
            }
            else
            {
                PlayerMovementEvent obj = new PlayerMovementEvent(game, clientPlayer, playerControl);
                items.Add(obj);
                counter++;
                return obj;
            }
        }

        private void Clear(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
        }

        public void Recycle()
        {
            if (counter < MAX)
            {
                items.Add(this);
                counter++;
            }
        }

        public void Dispose()
        {
            Recycle();
        }
    }
}
