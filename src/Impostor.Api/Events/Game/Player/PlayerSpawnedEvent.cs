using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public class PlayerSpawnedEvent : IPlayerEvent
    {
        public PlayerSpawnedEvent(IGame game, IClientPlayer clientPlayer, IInnerPlayerControl playerControl)
        {
            Game = game;
            ClientPlayer = clientPlayer;
            PlayerControl = playerControl;
        }

        /// <inheritdoc/>
        public IGame Game { get; }

        /// <inheritdoc/>
        public IClientPlayer ClientPlayer { get; }

        /// <inheritdoc/>
        public IInnerPlayerControl PlayerControl { get; }
    }
}