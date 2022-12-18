using Impostor.Api.Events.Player;
using Impostor.Api.Net.Inner.Objects;
using TVoteType = Impostor.Api.Events.Player.VoteType;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud
    {
        public class PlayerVoteArea : IInnerMeetingHud.IPlayerVoteArea
        {
            private byte _votedForId;

            public PlayerVoteArea(InnerMeetingHud parent, InnerPlayerInfo targetPlayer, bool isDead)
            {
                Parent = parent;
                TargetPlayer = targetPlayer;

                VotedForId = (byte)(isDead ? TVoteType.Dead : TVoteType.HasNotVoted);
            }

            public InnerMeetingHud Parent { get; }

            public InnerPlayerInfo TargetPlayer { get; }

            public bool IsDead => VoteType == TVoteType.Dead;

            public bool DidVote => VoteType != TVoteType.HasNotVoted;

            public bool DidReport { get; private set; }

            public byte VotedForId
            {
                get => _votedForId;

                private set
                {
                    _votedForId = value;

                    switch ((VoteType)value)
                    {
                        case TVoteType.Dead:
                            VoteType = TVoteType.Dead;
                            break;

                        case TVoteType.HasNotVoted:
                        case TVoteType.Missed:
                        case TVoteType.Skipped:
                            VoteType = (VoteType)value;
                            break;

                        default:
                            VoteType = TVoteType.Player;
                            VotedFor = Parent.Game.GameNet.GameData!.GetPlayerById(value)?.Controller;
                            break;
                    }
                }
            }

            public VoteType? VoteType { get; private set; }

            public IInnerPlayerControl? VotedFor { get; private set; }

            IInnerPlayerInfo IInnerMeetingHud.IPlayerVoteArea.TargetPlayer => TargetPlayer;

            internal void Deserialize(IMessageReader reader, bool updateVote)
            {
                var votedForId = reader.ReadByte();

                if (updateVote)
                {
                    VotedForId = votedForId;
                }

                DidReport = reader.ReadBoolean();
            }

            internal void SetVotedFor(byte votedFor)
            {
                VotedForId = votedFor;
            }
        }
    }
}
