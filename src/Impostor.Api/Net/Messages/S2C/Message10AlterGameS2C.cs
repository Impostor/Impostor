using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message10AlterGameS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, bool isPublic)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.AlterGame);
            writer.Write(gameCode);
            writer.Write((byte)AlterGameTags.ChangePrivacy);
            writer.Write(isPublic);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
