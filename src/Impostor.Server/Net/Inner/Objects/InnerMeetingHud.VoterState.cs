namespace Impostor.Server.Net.Inner.Objects;

internal partial class InnerMeetingHud
{
    public class VoterState(byte voterId, byte votedForId)
    {
        public byte VoterId { get; } = voterId;

        public byte VotedForId { get; } = votedForId;

        public static VoterState Deserialize(IMessageReader reader)
        {
            return new VoterState(reader.Tag, reader.ReadByte());
        }

        public void Serialize(IMessageWriter writer)
        {
            writer.StartMessage(VoterId);
            writer.Write(VotedForId);
            writer.EndMessage();
        }
    }
}
