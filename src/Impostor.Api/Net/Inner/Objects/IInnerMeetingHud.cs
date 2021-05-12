using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Impostor.Api.Events.Player;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerMeetingHud : IInnerNetObject
    {
        public interface IPlayerVoteArea
        {
            /// <summary>
            ///     Gets the player info of this vote state.
            /// </summary>
            IInnerPlayerInfo TargetPlayer { get; }

            /// <summary>
            ///     Gets a value indicating whether the player is a dead.
            /// </summary>
            bool IsDead { get; }

            /// <summary>
            ///     Gets a value indicating whether the player started this meeting.
            /// </summary>
            bool DidReport { get; }

            /// <summary>
            ///     Gets a value indicating whether the player voted.
            /// </summary>
            bool DidVote { get; }

            /// <summary>
            ///     Gets the vote type.
            /// </summary>
            /// <remarks>
            ///     Null when <see cref="DidVote" /> is false.
            /// </remarks>
            [MemberNotNullWhen(true, nameof(DidVote))]
            public VoteType? VoteType { get; }

            /// <summary>
            ///     Gets the player voted for.
            /// </summary>
            /// <remarks>
            ///     Null when <see cref="VoteType" /> isn't <see cref="F:Impostor.Api.Events.Player.VoteType.Player"/>.
            /// </remarks>
            public IInnerPlayerControl? VotedFor { get; }
        }

        /// <summary>
        ///     Gets states of players taking part in this meeting.
        /// </summary>
        IReadOnlyCollection<IPlayerVoteArea> PlayerStates { get; }

        /// <summary>
        ///     Gets the player that started the meeting.
        /// </summary>
        IInnerPlayerInfo? Reporter { get; }
    }
}
