using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Api.Events
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