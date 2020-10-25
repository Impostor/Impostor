using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public class PlayerChatEvent : IPlayerEvent
    {
        public PlayerChatEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl, string message)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
            Message = message;
        }

        /// <inheritdoc/>
        public IGame Game { get; }

        /// <inheritdoc/>
        public IClientPlayer ClientPlayer { get; }

        /// <inheritdoc/>
        public IInnerPlayerControl PlayerControl { get; }

        /// <summary>
        ///     Gets the message sent by the player.
        /// </summary>
        public string Message { get; }
    }
}