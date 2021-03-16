namespace Impostor.Api.Events.Player
{
    public interface IPlayerChatEvent : IPlayerEvent, IEventCancelable
    {
        /// <summary>
        ///     Gets the message sent by the player.
        /// </summary>
        string Message { get; }
    }
}
