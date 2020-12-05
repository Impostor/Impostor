namespace Impostor.Api.Events
{
    public interface IGamePlayerLeftEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer"/> which triggered the event.
        /// </summary>
        IClientPlayer Player { get; }
    }
}
