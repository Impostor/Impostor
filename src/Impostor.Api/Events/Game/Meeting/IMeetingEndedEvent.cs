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
    }
}
