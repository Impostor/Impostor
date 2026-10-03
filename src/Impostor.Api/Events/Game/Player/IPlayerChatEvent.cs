namespace Impostor.Api.Events.Player
{
    public interface IPlayerChatEvent : IPlayerEvent, IEventCancelable
    {
        /// <summary>
        ///     Gets the message sent by the player.
        /// </summary>
        string Message { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the message is sent to
        ///     all players (true) or only to the host (false).
        ///     To not send this message at all, cancel this event instead.
        /// </summary>
        bool SendToAllPlayers { get; set; }
    }
}
