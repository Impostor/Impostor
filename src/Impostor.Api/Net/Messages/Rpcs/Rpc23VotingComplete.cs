namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc23VotingComplete
    {
        public static void Serialize(IMessageWriter writer, byte[] states, byte playerId, bool tie, bool wasOverruled, ushort overrideId)
        {
            writer.WriteBytesAndSize(states);
            writer.Write(playerId);
            writer.Write(tie);
            writer.Write(wasOverruled);
            writer.Write(overrideId);
        }

        public static void Deserialize(IMessageReader reader, bool hasOverruleFields, out IMessageReader[] states, out byte playerId, out bool tie, out bool wasOverruled, out ushort overrideId)
        {
            var length = reader.ReadPackedInt32();
            states = new IMessageReader[length];
            for (var i = 0; i < length; i++)
            {
                states[i] = reader.ReadMessage();
            }

            playerId = reader.ReadByte();
            tie = reader.ReadBoolean();

            if (hasOverruleFields)
            {
                wasOverruled = reader.ReadBoolean();
                overrideId = reader.ReadUInt16();
            }
            else
            {
                wasOverruled = false;
                overrideId = 0;
            }
        }
    }
}
