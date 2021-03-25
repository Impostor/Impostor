using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerStartMeetingEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the player who's body got reported. Is null when the meeting started by Emergency call button.
        /// </summary>
        IInnerPlayerControl? Body { get; }
    }
}
