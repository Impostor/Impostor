namespace Impostor.Api.Events.Player
{
    public interface IPlayerSetStartCounterEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the current time of the start counter.
        /// </summary>
        byte SecondsLeft { get; }
    }
}
