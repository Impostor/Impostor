using System.IO;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class Message00HostGameC2S
    {
        public static void Serialize(IMessageWriter writer, GameOptionsData gameOptionsData)
        {
            writer.StartMessage(MessageFlags.HostGame);

            using (var memory = new MemoryStream())
            using (var writerBin = new BinaryWriter(memory))
            {
                gameOptionsData.Serialize(writerBin, GameOptionsData.LatestVersion);
                writer.WriteBytesAndSize(memory.ToArray());
            }

            writer.EndMessage();
        }

        public static GameOptionsData Deserialize(IMessageReader reader)
        {
            return GameOptionsData.Deserialize(reader.ReadBytesAndSize());
        }
    }
}