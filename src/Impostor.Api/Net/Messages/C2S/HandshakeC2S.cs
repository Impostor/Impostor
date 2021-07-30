using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class HandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out int clientVersion, out string name, out uint? lastNonceReceived, out Language language, out QuickChatModes chatMode)
        {
            clientVersion = reader.ReadInt32();
            name = reader.ReadString();

            if (clientVersion >= Version.V1)
            {
                lastNonceReceived = reader.ReadUInt32();
            }
            else
            {
                lastNonceReceived = null;
            }

            if (clientVersion >= Version.V2)
            {
                language = (Language)reader.ReadUInt32();
                chatMode = (QuickChatModes)reader.ReadByte();
            }
            else
            {
                language = Language.English;
                chatMode = QuickChatModes.FreeChatOrQuickChat;
            }
        }

        private static class Version
        {
            public static readonly int V1 = GameVersion.GetVersion(2021, 4, 25);

            public static readonly int V2 = GameVersion.GetVersion(2021, 6, 30);
        }
    }
}
