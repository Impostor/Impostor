using Impostor.Api.Events.Player;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using TVoteType = Impostor.Api.Events.Player.VoteType;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud
    {
        public class PlayerVoteArea : IInnerMeetingHud.IPlayerVoteArea
        {
            private const byte VoteMask = 15;
            private const byte ReportedBit = 32;
            private const byte VotedBit = 64;
            private const byte DeadBit = 128;

            private sbyte _votedForId;

            public PlayerVoteArea(InnerMeetingHud parent, InnerPlayerInfo targetPlayer, bool isDead)
            {
                Parent = parent;
                TargetPlayer = targetPlayer;

                IsDead = isDead;
            }

            public InnerMeetingHud Parent { get; }

            public InnerPlayerInfo TargetPlayer { get; }

            public bool IsDead { get; private set; }

            public bool DidVote { get; private set; }

            public bool DidReport { get; private set; }

            public sbyte VotedForId
            {
                get => _votedForId;

                private set
                {
                    _votedForId = value;

                    if (DidVote)
                    {
                        switch ((VoteType)value)
                        {
                            case TVoteType.ForceSkip:
                            case TVoteType.Skip:
                                VoteType = (VoteType)value;
                                break;

                            default:
                                VoteType = TVoteType.Player;
                                VotedFor = Parent.Game.GameNet.GameData!.GetPlayerById((byte)value)?.Controller;
                                break;
                        }
                    }
                    else
                    {
                        VoteType = null;
                        VotedFor = null;
                    }
                }
            }

            public VoteType? VoteType { get; private set; }

            public IInnerPlayerControl? VotedFor { get; private set; }

            IInnerPlayerInfo IInnerMeetingHud.IPlayerVoteArea.TargetPlayer => TargetPlayer;

            internal void Deserialize(IMessageReader reader, bool updateVote)
            {
                var state = reader.ReadByte();
                DeserializeState(state, out var votedForId, out var isDead, out var didVote, out var didReport);

                IsDead = isDead;
                DidReport = didReport;

                if (updateVote)
                {
                    DidVote = didVote;
                    VotedForId = votedForId;
                }
            }

            internal void SetVotedFor(sbyte votedFor)
            {
                DidVote = true;
                VotedForId = votedFor;
            }

            private static void DeserializeState(byte state, out sbyte votedForId, out bool isDead, out bool didVote, out bool didReport)
            {
                votedForId = (sbyte)((state & VoteMask) - 1);

                // Among Us's meeting system is really scuffed, we need this to ensure that ForceSkip state is correct (this will break any 14+ players game)
                if (votedForId == ((sbyte)TVoteType.ForceSkip + 1 & 14))
                {
                    votedForId = (sbyte)TVoteType.ForceSkip;
                }

                isDead = (state & DeadBit) > 0;
                didVote = (state & VotedBit) > 0;
                didReport = (state & ReportedBit) > 0;
            }
        }
    }
}
