namespace Impostor.Api.Net.Messages.Announcements
{
    public static class Message00UseCache
    {
        public static void Serialize(IMessageWriter writer)
        {
            writer.StartMessage(AnnouncementsMessageFlags.UseCache);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
        }
    }
}
