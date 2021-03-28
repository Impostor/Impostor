using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud
    {
        public class PlayerVoteArea
        {
            private const byte VoteMask = 15;
            private const byte ReportedBit = 32;
            private const byte VotedBit = 64;
            private const byte DeadBit = 128;

            public PlayerVoteArea(InnerMeetingHud parent, byte targetPlayerId)
            {
                Parent = parent;
                TargetPlayerId = targetPlayerId;
            }

            public InnerMeetingHud Parent { get; }

            public byte TargetPlayerId { get; }

            public bool IsDead { get; private set; }

            public bool DidVote { get; private set; }

            public bool DidReport { get; private set; }

            public sbyte VotedFor { get; private set; }

            public void Deserialize(IMessageReader reader)
            {
                var num = reader.ReadByte();

                VotedFor = (sbyte)((num & VoteMask) - 1);
                IsDead = (num & DeadBit) > 0;
                DidVote = (num & VotedBit) > 0;
                DidReport = (num & ReportedBit) > 0;
            }

            internal void SetDead(bool didReport, bool isDead)
            {
                DidReport = didReport;
                IsDead = isDead;
            }
        }
    }
}
