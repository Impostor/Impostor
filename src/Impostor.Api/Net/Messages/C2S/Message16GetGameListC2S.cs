using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message16GetGameListC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out GameOptionsData options)
        {
            reader.ReadPackedInt32(); // Hardcoded 0.
            options = GameOptionsData.Deserialize(reader.ReadBytesAndSize());
        }
    }
}