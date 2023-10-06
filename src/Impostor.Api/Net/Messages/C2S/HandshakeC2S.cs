using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class HandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out GameVersion clientVersion, out string name, out Language language, out QuickChatModes chatMode, out PlatformSpecificData? platformSpecificData)
        {
            clientVersion = reader.ReadGameVersion();
            name = reader.ReadString();

            if (clientVersion >= Version.V1)
            {
                reader.ReadUInt32(); // lastNonceReceived, always 0 since 2021.11.9
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

            if (clientVersion >= Version.V3)
            {
                using var platformReader = reader.ReadMessage();
                platformSpecificData = new PlatformSpecificData(platformReader);
                reader.ReadInt32(); // crossplayFlags, not used yet
            }
            else
            {
                platformSpecificData = null;
            }

            if (clientVersion >= Version.V4)
            {
                reader.ReadByte(); // purpose unknown, seems hardcoded to 0
            }
        }

        private static class Version
        {
            public static readonly GameVersion V1 = new(2021, 4, 25);

            public static readonly GameVersion V2 = new(2021, 6, 30);

            public static readonly GameVersion V3 = new(2021, 11, 9);

            public static readonly GameVersion V4 = new(2021, 12, 14);
        }
    }
}
