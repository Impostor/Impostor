using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    /// <summary>
    /// Event that is called when a player is killed by another player.
    /// </summary>
    /// <remarks>
    /// This event works regardless of server authority is enabled or not.
    ///
    /// If you want to cancel this kill, listen to <see cref="IPlayerCheckMurderEvent"/>.
    /// If you want to know about players that were voted out, listed to <see cref="IPlayerExileEvent"/>.
    /// </remarks>
    public interface IPlayerMurderEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the player who got murdered.
        /// </summary>
        IInnerPlayerControl Victim { get; }

        /// <summary>
        /// Gets the result of the event.
        /// </summary>
        /// <remarks>
        /// Note that if FailedError or FailedProtected is set, the kill did not take place.
        /// </remarks>
        MurderResultFlags Result { get; }
    }
}
