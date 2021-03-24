using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called whenever a new <see cref="IGame" /> is destroyed.
    /// </summary>
    public interface IGameDestroyedEvent : IGameEvent
    {
    }
}
