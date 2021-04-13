using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Auth
{
    public static class MessageHandshake
    {
        public static void Deserialize(IMessageReader reader, out int clientVersion, out Platforms platform, out string clientId)
        {
            clientVersion = reader.ReadInt32();
            platform = (Platforms)reader.ReadByte();
            clientId = reader.ReadString();
        }
    }
}
