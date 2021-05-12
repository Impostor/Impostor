using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public enum VoteType : sbyte
    {
        ForceSkip = -2,
        Skip = -1,
        Player = 0,
    }

    public interface IPlayerVotedEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the player he voted for.
        /// </summary>
        IInnerPlayerControl? VotedFor { get; }

        /// <summary>
        ///     Gets the Vote type.
        /// </summary>
        VoteType VoteType { get; }
    }
}
