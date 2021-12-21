using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public class MessageDisconnect
    {
        public static void Serialize(IMessageWriter writer, bool hasReason, DisconnectReason? reason, string? message)
        {
            writer.Write(hasReason);

            if (hasReason)
            {
                if (reason == null)
                {
                    throw new ArgumentNullException(nameof(reason));
                }

                writer.StartMessage(0);
                writer.Write((byte)reason);

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
        }

        public static void Deserialize(IMessageReader reader, out bool hasReason, out DisconnectReason? reason, out string? message)
        {
            hasReason = reader.ReadBoolean();

            if (hasReason)
            {
                using var inner = reader.ReadMessage();
                reason = (DisconnectReason)inner.ReadByte();
                message = reason == DisconnectReason.Custom ? inner.ReadString() : null;
            }
            else
            {
                reason = null;
                message = null;
            }
        }
    }
}
