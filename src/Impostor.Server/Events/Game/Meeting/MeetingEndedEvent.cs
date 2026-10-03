using Impostor.Api.Events.Meeting;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Meeting
{
    public class MeetingEndedEvent : IMeetingEndedEvent
    {
        public MeetingEndedEvent(IGame game, IInnerMeetingHud meetingHud, IInnerPlayerControl? exiled, bool isTie, bool wasOverruled = false, ushort overrideId = 0)
        {
            Game = game;
            MeetingHud = meetingHud;
            Exiled = exiled;
            IsTie = isTie;
            WasOverruled = wasOverruled;
            OverrideId = overrideId;
        }

        public IGame Game { get; }

        public IInnerMeetingHud MeetingHud { get; }

        public IInnerPlayerControl? Exiled { get; }

        public bool IsTie { get; }

        public bool WasOverruled { get; }

        public ushort OverrideId { get; }
    }
}
