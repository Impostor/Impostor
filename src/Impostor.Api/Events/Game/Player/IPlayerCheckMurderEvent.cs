using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    /// <summary>
    /// Event that allows changing or canceling an upcoming player murder.
    /// </summary>
    /// <remarks>
    /// Note that this event only triggers if the Host of the Game did not disable server authority.
    ///
    /// If you want to get an event after the murder took place, listen to <see cref="IPlayerMurderEvent"/>.
    /// </remarks>
    public interface IPlayerCheckMurderEvent : IPlayerEvent, IEventCancelable
    {
        /// <summary>
        /// Gets the player who will be murdered.
        /// </summary>
        IInnerPlayerControl Victim { get; }

        /// <summary>
        /// Gets or sets the result of this event.
        /// </summary>
        /// <remarks>
        /// Its initial value is what would have happened in the normal game flow.
        /// - If set to Succeeded, the Victim will die.
        /// - If set to FailedProtected, the Victim will be protected with a shield animation and the attacker will have a cooldown set.
        /// </remarks>
        MurderResultFlags Result { get; set; }
    }
}
