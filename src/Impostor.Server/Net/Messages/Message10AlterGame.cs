using Hazel;

namespace Impostor.Server.Net.Messages
{
    internal static class Message10AlterGame
    {
        public static void Serialize(MessageWriter writer, bool clear, int gameCode)
        {
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.HostGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }
    }
}