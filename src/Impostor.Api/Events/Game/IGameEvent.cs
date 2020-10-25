using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    public interface IGameEvent : IEvent
    {
        IGame Game { get; }
    }
}