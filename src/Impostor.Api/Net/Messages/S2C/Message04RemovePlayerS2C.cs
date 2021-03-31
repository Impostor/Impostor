using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message04RemovePlayerS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, int playerId, int hostId, DisconnectReason reason)
        {
            // Only a subset of DisconnectReason shows an unique message.
            // ExitGame, Banned and Kicked.
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.RemovePlayer);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.Write(hostId);
            writer.Write((byte)reason);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
