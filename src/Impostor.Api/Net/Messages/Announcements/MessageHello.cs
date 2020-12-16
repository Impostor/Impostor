using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Announcements
{
    public class MessageHello
    {
        public static void Deserialize(IMessageReader reader, out int announcementVersion, out int id, out Language language)
        {
            reader.ReadByte(); // TODO the hec is this
            announcementVersion = reader.ReadPackedInt32();
            id = reader.ReadPackedInt32();
            language = (Language)reader.ReadPackedInt32();
        }
    }
}
