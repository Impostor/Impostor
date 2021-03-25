using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message01JoinGameS2C
    {
        public static void SerializeJoin(IMessageWriter writer, bool clear, int gameCode, int playerId, int hostId)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.JoinGame);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.Write(hostId);
            writer.EndMessage();
        }

        public static void SerializeError(IMessageWriter writer, bool clear, DisconnectReason reason, string? message = null)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.JoinGame);
            writer.Write((int)reason);

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

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
