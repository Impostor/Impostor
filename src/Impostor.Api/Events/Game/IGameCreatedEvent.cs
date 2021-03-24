using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called whenever a new <see cref="IGame" /> is created.
    /// </summary>
    public interface IGameCreatedEvent : IGameEvent
    {
    }
}
