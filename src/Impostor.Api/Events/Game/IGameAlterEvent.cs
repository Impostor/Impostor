namespace Impostor.Api.Events
{
    public interface IGameAlterEvent : IGameEvent
    {
        bool IsPublic { get; }
    }
}
