using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerCompletedTaskEvent : IPlayerEvent, Impostor.Api.Events.IEventCancelable
    {
        ITaskInfo Task { get; }
    }
}
