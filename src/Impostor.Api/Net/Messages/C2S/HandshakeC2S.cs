using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class HandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out int clientVersion, out string name, out uint lastNonceReceived, out Language language, out QuickChatModes chatMode)
        {
            clientVersion = reader.ReadInt32();
            name = reader.ReadString();
            lastNonceReceived = reader.ReadUInt32();

            // Version 2021.6.30 (aka 50537300) is the first version that includes language and chat modes
            // In case the game is older than that, stop reading here.
            if (clientVersion >= 50537300)
            {
                language = (Language)reader.ReadUInt32();
                chatMode = (QuickChatModes)reader.ReadByte();
            }
            else
            {
                // This is an old version of Among Us and will fail later in the handshake due to a version mismatch
                language = Language.English;
                chatMode = QuickChatModes.FreeChatOrQuickChat;
            }
        }
    }
}
