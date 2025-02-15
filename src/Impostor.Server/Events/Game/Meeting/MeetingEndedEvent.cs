using Impostor.Api.Events.Meeting;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Meeting;

public class MeetingEndedEvent(IGame game, IInnerMeetingHud meetingHud, IInnerPlayerControl? exiled, bool isTie)
    : IMeetingEndedEvent
{
    public IGame Game { get; } = game;

    public IInnerMeetingHud MeetingHud { get; } = meetingHud;

    public IInnerPlayerControl? Exiled { get; } = exiled;

    public bool IsTie { get; } = isTie;
}
