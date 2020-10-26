using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Meeting
{
    public interface IMeetingEvent : IGameEvent
    {
        IInnerMeetingHud MeetingHud { get; }
    }
}
