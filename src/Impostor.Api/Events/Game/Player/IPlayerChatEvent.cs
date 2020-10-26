namespace Impostor.Api.Events.Player
{
    public interface IPlayerChatEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the message sent by the player.
        /// </summary>
        string Message { get; }
    }
}
