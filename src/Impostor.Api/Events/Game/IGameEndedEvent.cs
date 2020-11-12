using Impostor.Api.Innersloth;

namespace Impostor.Api.Events
{
    public interface IGameEndedEvent : IGameEvent
    {
        public GameOverReason GameOverReason { get; }
    }
}
