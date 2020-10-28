namespace Impostor.Api.Events.Player
{
    public interface IPlayerCompletedTaskEvent : IPlayerEvent
    {
        uint TaskID { get; }
    }
}
