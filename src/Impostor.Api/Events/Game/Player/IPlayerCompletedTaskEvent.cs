using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerCompletedTaskEvent : IPlayerEvent
    {
        uint TaskID { get; }

        TaskTypes Task { get; }
    }
}