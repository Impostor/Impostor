using Impostor.Api.Events.Meeting;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Meeting;

public class MeetingStartedEvent(IGame game, IInnerMeetingHud meetingHud) : IMeetingStartedEvent
{
    public IGame Game { get; } = game;

    public IInnerMeetingHud MeetingHud { get; } = meetingHud;
}
