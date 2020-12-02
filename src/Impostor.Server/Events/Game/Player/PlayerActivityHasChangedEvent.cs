using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Player
{
    public class PlayerActivityHasChangedEvent : IPlayerActivityHasChangedEvent
    {
        public PlayerActivityHasChangedEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, ActivityType previousActivity, ActivityType currentActivity)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            PreviousActivity = previousActivity;
            CurrentActivity = currentActivity;
        }

        public IGame Game { get; }

        public IClientPlayer ClientPlayer { get; }

        public IInnerPlayerControl PlayerControl { get; }

        public ActivityType PreviousActivity { get; }
        public ActivityType CurrentActivity { get; }
    }
}
