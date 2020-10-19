using Impostor.Server.Games;
using Impostor.Server.Net;

namespace Impostor.Server.Events
{
    public class PlayerJoinedGameEvent : IGameEvent
    {
        public PlayerJoinedGameEvent(IGame game, IClientPlayer player)
        {
            Game = game;
            Player = player;
        }

        public IGame Game { get; }

        public IClientPlayer Player { get; }
    }
}