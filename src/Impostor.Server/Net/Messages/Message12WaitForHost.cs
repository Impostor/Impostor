using Hazel;

namespace Impostor.Server.Net.Messages
{
    internal static class Message12WaitForHost
    {
        public static void Serialize(MessageWriter writer, bool clear, int gameCode, int playerId)
        {
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.WaitForHost);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.EndMessage();
        }
    }
}