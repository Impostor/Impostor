using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example.Handlers
{
    public class MeetingEventListener : IEventListener
    {
        private readonly ILogger<MeetingEventListener> _logger;

        public MeetingEventListener(ILogger<MeetingEventListener> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent e)
        {
            _logger.LogInformation("Meeting > started");
        }

        [EventListener]
        public void OnMeetingEnded(IMeetingEndedEvent e)
        {
            _logger.LogInformation("Meeting > ended, exiled: {exiled}, tie: {tie}", e.Exiled?.PlayerInfo.PlayerName, e.IsTie);

            foreach (var playerState in e.MeetingHud.PlayerStates)
            {
                if (playerState.IsDead)
                {
                    _logger.LogInformation("- {player} is dead", playerState.TargetPlayer.PlayerName);
                }
                else
                {
                    _logger.LogInformation("- {player} voted for {voteType} {votedFor}", playerState.TargetPlayer.PlayerName, playerState.VoteType, playerState.VotedFor?.PlayerInfo.PlayerName);
                }
            }
        }
    }
}
