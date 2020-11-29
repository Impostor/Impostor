using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerWatchingCameraEvent : IPlayerWatchingCameraEvent
    {
        public PlayerWatchingCameraEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, bool started)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            Started = started;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public bool Started { get; }
    }
}
