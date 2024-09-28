namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc40SetSkinStr
    {
        public static void Serialize(IMessageWriter writer, string skin, byte nextRpcSequenceId)
        {
            writer.Write(skin);
            writer.Write(nextRpcSequenceId);
        }

        public static void Deserialize(IMessageReader reader, out string skin, out byte nextRpcSequenceId)
        {
            skin = reader.ReadString();
            nextRpcSequenceId = reader.ReadByte();
        }
    }
}
