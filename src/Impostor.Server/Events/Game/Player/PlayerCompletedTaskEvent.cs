using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerCompletedTaskEvent : IPlayerCompletedTaskEvent
    {
        public PlayerCompletedTaskEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, ITaskInfo task)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            Task = task;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public ITaskInfo Task { get; }
    }
}
