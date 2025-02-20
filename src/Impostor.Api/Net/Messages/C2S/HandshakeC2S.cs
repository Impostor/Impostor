using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S;

public static class HandshakeC2S
{
    public static void Deserialize(
        IMessageReader reader, 
        bool isDtl,
        out GameVersion clientVersion,
        out string name,
        out Language language,
        out QuickChatModes chatMode,
        out PlatformSpecificData? platformSpecificData,
        out string matchmakerToken,
        out uint lastId,
        out string friendCode)
    {
        clientVersion = reader.ReadGameVersion();
        name = reader.ReadString();

        if (isDtl)
        {
            matchmakerToken = reader.ReadString();
            lastId = 0;
        }
        else
        {
            matchmakerToken = string.Empty;
            lastId = reader.ReadUInt32();
        }
        
        language = (Language)reader.ReadUInt32();
        chatMode = (QuickChatModes)reader.ReadByte();

        using var platformReader = reader.ReadMessage();
        platformSpecificData = new PlatformSpecificData(platformReader);

        if (isDtl)
        {
            friendCode = reader.ReadString();
        }
        else
        {
            friendCode = string.Empty;
            reader.ReadString();
            reader.ReadUInt32();
        }
        
        /*reader.ReadInt32(); // crossplayFlags, not used yet
        
        reader.ReadByte(); // purpose unknown, seems hardcoded to 0*/
    }
}
