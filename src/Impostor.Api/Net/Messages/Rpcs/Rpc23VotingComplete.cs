namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc23VotingComplete
    {
        public static void Serialize(IMessageWriter writer, byte[] states, byte playerId, bool tie)
        {
            writer.WriteBytesAndSize(states);
            writer.Write(playerId);
            writer.Write(tie);
        }

        public static void Deserialize(IMessageReader reader, out IMessageReader[] states, out byte playerId, out bool tie)
        {
            var length = reader.ReadPackedInt32();
            states = new IMessageReader[length];
            for (var i = 0; i < length; i++)
            {
                states[i] = reader.ReadMessage();
            }

            playerId = reader.ReadByte();
            tie = reader.ReadBoolean();
        }
    }
}
