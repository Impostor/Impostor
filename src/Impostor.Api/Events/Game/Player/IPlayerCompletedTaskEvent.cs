using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerCompletedTaskEvent : IPlayerEvent
    {
        TaskTypes Task { get; }
    }
}