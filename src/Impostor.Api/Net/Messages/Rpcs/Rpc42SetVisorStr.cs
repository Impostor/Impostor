namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc42SetVisorStr
    {
        public static void Serialize(IMessageWriter writer, string visor)
        {
            writer.Write(visor);
        }

        public static void Deserialize(IMessageReader reader, out string visor)
        {
            visor = reader.ReadString();
        }
    }
}
