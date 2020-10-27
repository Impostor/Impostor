using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerMurderEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the player who got murdered.
        /// </summary>
        IInnerPlayerControl Victim { get; }
    }
}
