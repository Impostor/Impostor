using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S;

public static class AuthHandshakeC2S
{
    public static void Deserialize(IMessageReader reader, 
        out GameVersion clientVersion, out Platforms platforms,
        out string matchmakerToken, out string friendCode
        )
    {
        clientVersion = reader.ReadGameVersion();
        platforms = (Platforms)reader.ReadByte();
        matchmakerToken = reader.ReadString();
        friendCode = reader.ReadString();
    }
}
