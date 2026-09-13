using Impostor.Api.Innersloth;

namespace Impostor.Api.Events
{
    public interface IGameEndedEvent : IGameEvent, IEventCancelable
    {
        public GameOverReason GameOverReason { get; }
    }
}
