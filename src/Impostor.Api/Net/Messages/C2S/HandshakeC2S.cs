using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class HandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out GameVersion clientVersion, out string name, out Language language, out QuickChatModes chatMode, out PlatformSpecificData? platformSpecificData)
        {
            clientVersion = reader.ReadGameVersion();

            if (clientVersion < Version.V3)
            {
                throw new ImpostorProtocolException("Client version is too old");

                // Before 2021.11.9, handshake uses BinaryWriter which can not be deserialized as a message reader.
            }

            name = reader.ReadString();

            reader.ReadUInt32(); // lastNonceReceived, always 0 since 2021.11.9

            language = (Language)reader.ReadUInt32();
            chatMode = (QuickChatModes)reader.ReadByte();

            using var platformReader = reader.ReadMessage();
            platformSpecificData = new PlatformSpecificData(platformReader);

            if (clientVersion.Normalize() == Version.V3)
            {
                // Crossplay flag was removed in 2021.12.14, friendcode was added instead
                reader.ReadInt32();
            }
            else
            {
                reader.ReadString(); // friendcode placeholder, always empty since 2021.12.14
                reader.ReadUInt32();
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
