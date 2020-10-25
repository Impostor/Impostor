using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Meeting
{
    public class MeetingStartedEvent : IEvent
    {
        public MeetingStartedEvent(IGame game, IInnerMeetingHud meetingHud)
        {
            Game = game;
            MeetingHud = meetingHud;
        }

        public IGame Game { get; }

        public IInnerMeetingHud MeetingHud { get; }
    }
}