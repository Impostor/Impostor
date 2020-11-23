using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerVotedEvent : IPlayerEvent
    {
        /// <summary>
        ///     Get the player he voted for.
        /// </summary>
        IInnerPlayerControl? VotedFor { get; }
    }
}
