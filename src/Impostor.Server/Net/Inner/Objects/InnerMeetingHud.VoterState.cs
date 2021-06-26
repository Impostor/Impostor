using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud
    {
        public class VoterState
        {
            public VoterState(byte voterId, byte votedForId)
            {
                VoterId = voterId;
                VotedForId = votedForId;
            }

            public byte VoterId { get; }

            public byte VotedForId { get; }

            public static VoterState Deserialize(IMessageReader reader)
            {
                return new VoterState(reader.Tag, reader.ReadByte());
            }

            public void Serialize(IMessageWriter writer)
            {
                writer.StartMessage(this.VoterId);
                writer.Write(this.VotedForId);
                writer.EndMessage();
            }
        }
    }
}
