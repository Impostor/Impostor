using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages
{
    internal static class Message06GameData
    {

        public static void Deserialize(MessageReader reader, out int contentSize, out byte unknown, out byte gameDataType)
        {
            contentSize = reader.ReadPackedInt32();
            unknown = reader.ReadByte();
            gameDataType = reader.ReadByte();
        }
    }
}