using Impostor.Server.Games;
using Impostor.Server.Net;

namespace Impostor.Server.Events
{
    public class PlayerLeftGameEvent : IGameEvent
    {
        public PlayerLeftGameEvent(IGame game, IClientPlayer player, bool isBan)
        {
            Game = game;
            Player = player;
            IsBan = isBan;
        }

        public IGame Game { get; }

        public IClientPlayer Player { get; }

        public bool IsBan { get; }
    }
}