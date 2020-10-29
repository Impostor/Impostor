using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerReportedBodyEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the player who's body got reported.
        ///     Body can be null if the report happens too quickly.
        /// </summary>
        IInnerPlayerControl Body { get; }
    }
}
