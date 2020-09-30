using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages
{
    internal static class Message04RemovePlayer
    {
        public static void Serialize(MessageWriter writer, bool clear, int gameCode, int playerId, int hostId, DisconnectReason reason)
        {
            // Only a subset of DisconnectReason shows an unique message.
            // ExitGame, Banned and Kicked.
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.RemovePlayer);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.Write(hostId);
            writer.Write((byte) reason);
            writer.EndMessage();
        }

        public static void Deserialize(MessageReader reader, out int playerId, out byte reason)
        {
            playerId = reader.ReadPackedInt32();
            reason = reader.ReadByte();
        }
    }
}