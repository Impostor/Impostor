using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Announcements
{
    public static class Message02SetFreeWeekend
    {
        public static void Serialize(IMessageWriter writer, FreeWeekendState state)
        {
            writer.StartMessage(AnnouncementsMessageFlags.SetFreeWeekend);
            writer.Write((byte)state);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out FreeWeekendState state)
        {
            state = (FreeWeekendState)reader.ReadByte();
        }
    }
}
