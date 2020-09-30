using System;
using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages
{
    internal static class Message01JoinGame
    {
        public static void SerializeJoin(MessageWriter writer, bool clear, int gameCode, int playerId, int hostId)
        {
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.JoinGame);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.Write(hostId);
            writer.EndMessage();
        }
        
        public static void SerializeError(MessageWriter writer, bool clear, DisconnectReason reason, string message = null)
        {
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.JoinGame);
            writer.Write((int) reason);

            if (reason == DisconnectReason.Custom)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }
                
                writer.Write(message);
            }
            
            writer.EndMessage();
        }

        public static void Deserialize(MessageReader reader, out int gameCode, out byte unknown)
        {
            gameCode = reader.ReadInt32();
            unknown = reader.ReadByte();
        }
    }
}