namespace Impostor.Api.Net.Messages.C2S
{
    public static class HandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out int clientVersion, out string name, out uint lastNonceReceived)
        {
            clientVersion = reader.ReadInt32();
            name = reader.ReadString();
            lastNonceReceived = reader.ReadUInt32();
        }
    }
}
