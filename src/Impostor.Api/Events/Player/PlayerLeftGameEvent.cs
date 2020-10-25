using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Api.Events.Player
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