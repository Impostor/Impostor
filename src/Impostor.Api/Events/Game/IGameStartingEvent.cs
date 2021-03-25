namespace Impostor.Api.Events
{
    /// <summary>
    ///     Called when the game is going to start.
    ///     When this is called, not all players are initialized properly yet.
    ///     If you want to get correct player states, use <see cref="IGameStartedEvent" />.
    /// </summary>
    public interface IGameStartingEvent : IGameEvent
    {
    }
}
