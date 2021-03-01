namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc15SetScanner
    {
        public static void Serialize(IMessageWriter writer, bool on, byte scannerCount)
        {
            writer.Write(on);
            writer.Write(scannerCount);
        }

        public static void Deserialize(IMessageReader reader, out bool on, out byte scannerCount)
        {
            on = reader.ReadBoolean();
            scannerCount = reader.ReadByte();
        }
    }
}
