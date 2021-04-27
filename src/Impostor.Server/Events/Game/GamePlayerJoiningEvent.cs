using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events
{
    public class GamePlayerJoiningEvent : IGamePlayerJoiningEvent
    {
        public GamePlayerJoiningEvent(IGame game, IClientPlayer player)
        {
            Game = game;
            Player = player;
        }

        public IGame Game { get; }

        public IClientPlayer Player { get; }

        public GameJoinResult? JoinResult { get; set; }
    }
}
