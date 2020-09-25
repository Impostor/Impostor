using Hazel;
using Impostor.Shared.Innersloth;

namespace Impostor.Server.Net.Messages
{
    internal static class Message00HostGame
    {
        public static void Serialize(MessageWriter writer, int gameCode)
        {
            writer.StartMessage(MessageFlags.HostGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }

        public static GameOptionsData Deserialize(MessageReader reader)
        {
            return GameOptionsData.Deserialize(reader.ReadBytesAndSize());
        }
    }
}