namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc43SetNamePlateStr
    {
        public static void Serialize(IMessageWriter writer, string namePlate)
        {
            writer.Write(namePlate);
        }

        public static void Deserialize(IMessageReader reader, out string namePlate)
        {
            namePlate = reader.ReadString();
        }
    }
}
