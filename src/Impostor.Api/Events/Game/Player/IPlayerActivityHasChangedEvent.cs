using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerActivityHasChangedEvent : IPlayerEvent
    {
        ActivityType PreviousActivity { get; }
        ActivityType CurrentActivity { get; }
    }
}
