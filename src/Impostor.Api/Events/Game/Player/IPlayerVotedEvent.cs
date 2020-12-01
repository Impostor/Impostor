using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerVotedEvent : IPlayerEvent
    {
        /// <summary>
        ///     Get the player he voted for.
        /// </summary>
        IInnerPlayerControl? VotedFor { get; }

        /// <summary>
        ///     Get the Vote type
        /// </summary>
        VoteType VoteType { get; }
    }

    public enum VoteType : sbyte {
        Skip = -1,
        Player = 0,
        None = 0xf-1
    }
}
