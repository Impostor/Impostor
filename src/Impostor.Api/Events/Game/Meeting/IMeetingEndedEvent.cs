using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Meeting
{
    public interface IMeetingEndedEvent : IMeetingEvent
    {
        /// <summary>
        ///     Gets the exiled player.
        /// </summary>
        IInnerPlayerControl? Exiled { get; }

        /// <summary>
        ///     Gets a value indicating whether meeting result is a tie.
        /// </summary>
        bool IsTie { get; }

        /// <summary>
        ///     Gets a value indicating whether a Judge overruled the vote result.
        /// </summary>
        bool WasOverruled { get; }

        /// <summary>
        ///     Gets the nonce of the winning Judge overrule, or 0 if the meeting wasn't overruled.
        /// </summary>
        ushort OverrideId { get; }
    }
}
