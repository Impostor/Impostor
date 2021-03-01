namespace Impostor.Api.Net.Messages.Announcements
{
    public static class Message01Update
    {
        public static void Serialize(IMessageWriter writer, int id, string message)
        {
            writer.StartMessage(AnnouncementsMessageFlags.SetUpdate);
            writer.WritePacked(id);
            writer.Write(message);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out int id, out string message)
        {
            id = reader.ReadPackedInt32();
            message = reader.ReadString();
        }
    }
}
