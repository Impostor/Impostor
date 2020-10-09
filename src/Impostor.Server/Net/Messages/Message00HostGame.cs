using Impostor.Shared.Innersloth;

namespace Impostor.Server.Net.Messages
{
    internal static class Message00HostGame
    {
        public static void Serialize(IMessageWriter writer, int gameCode)
        {
            writer.StartMessage(MessageFlags.HostGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }

        public static GameOptionsData Deserialize(IMessageReader reader)
        {
            return GameOptionsData.Deserialize(reader.ReadBytesAndSize());
        }
    }
}