using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events
{
    public class MeetingEndedEvent : IEvent
    {
        public MeetingEndedEvent(IGame game, IInnerMeetingHud meetingHud)
        {
            Game = game;
            MeetingHud = meetingHud;
        }

        public IGame Game { get; }

        public IInnerMeetingHud MeetingHud { get; }
    }
}