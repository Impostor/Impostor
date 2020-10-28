using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerReportedBodyEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the player who's body got reported.
        /// </summary>
        IInnerPlayerControl Body { get; }
    }
}
