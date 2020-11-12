namespace Impostor.Api.Events
{
    public interface IGameAlterEvent : IGameEvent, ICancellableEvent
    {
        bool IsPublic { get; }
    }
}
