using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerCompletedTaskEvent : IPlayerEvent
    {
        ITaskInfo Task { get; }
    }
}
