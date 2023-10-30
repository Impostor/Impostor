namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc40SetSkinStr
    {
        public static void Serialize(IMessageWriter writer, string skin)
        {
            writer.Write(skin);
        }

        public static void Deserialize(IMessageReader reader, out string skin)
        {
            skin = reader.ReadString();
        }
    }
}
