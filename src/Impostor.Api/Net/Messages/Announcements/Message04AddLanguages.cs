using System.Collections.Generic;

namespace Impostor.Api.Net.Messages.Announcements
{
    public static class Message04AddLanguages
    {
        public static void Serialize(IMessageWriter writer, Dictionary<string, uint> languages)
        {
            writer.StartMessage(AnnouncementsMessageFlags.AddLanguages);

            writer.Write((byte)languages.Count);
            foreach (var (name, id) in languages)
            {
                writer.Write(name);
                writer.Write(id);
            }

            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out Dictionary<string, uint> languages)
        {
            var length = reader.ReadByte();
            languages = new Dictionary<string, uint>(length);

            for (var i = 0; i < length; i++)
            {
                languages[reader.ReadString()] = reader.ReadUInt32();
            }
        }
    }
}
