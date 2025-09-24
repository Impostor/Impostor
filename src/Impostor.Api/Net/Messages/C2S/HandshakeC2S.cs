using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class HandshakeC2S
    {
        public static void Deserialize(IMessageReader reader, out GameVersion clientVersion, out string name, out Language language, out QuickChatModes chatMode, out PlatformSpecificData? platformSpecificData)
        {
            clientVersion = reader.ReadGameVersion();
            name = reader.ReadString();

            reader.ReadUInt32(); // lastNonceReceived, always 0 since 2021.11.9

            language = (Language)reader.ReadUInt32();
            chatMode = (QuickChatModes)reader.ReadByte();

            using var platformReader = reader.ReadMessage();
            platformSpecificData = new PlatformSpecificData(platformReader);

            // Skip the rest of the message (empty friendcode and placeholder uint 0)
        }
    }
}
