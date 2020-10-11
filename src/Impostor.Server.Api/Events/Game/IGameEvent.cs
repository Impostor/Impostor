using Impostor.Server.Games;

namespace Impostor.Server.Events
{
    public interface IGameEvent : IEvent
    {
        IGame Game { get; }
    }
}