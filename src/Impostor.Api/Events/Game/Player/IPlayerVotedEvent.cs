using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public enum VoteType : byte
    {
        HasNotVoted = 255,
        Missed = 254,
        Skipped = 253,
        Dead = 252,
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
