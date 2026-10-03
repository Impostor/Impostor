namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc66QueueOverruleVotes
    {
        public static void Serialize(IMessageWriter writer, byte judgePlayerId, byte targetPlayerId, ushort overruleNonce)
        {
            writer.Write(judgePlayerId);
            writer.Write(targetPlayerId);
            writer.Write(overruleNonce);
        }

        public static void Deserialize(IMessageReader reader, out byte judgePlayerId, out byte targetPlayerId, out ushort overruleNonce)
        {
            judgePlayerId = reader.ReadByte();
            targetPlayerId = reader.ReadByte();
            overruleNonce = reader.ReadUInt16();
        }
    }
}
