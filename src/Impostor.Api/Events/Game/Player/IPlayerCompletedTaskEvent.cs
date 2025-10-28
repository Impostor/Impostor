using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerCompletedTaskEvent : IPlayerEvent, IEventCancelable
    {
        ITaskInfo Task { get; }
    }
}
