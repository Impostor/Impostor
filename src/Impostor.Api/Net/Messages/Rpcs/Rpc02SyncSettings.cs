using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc02SyncSettings
    {
        public static void Serialize(IMessageWriter writer, GameOptionsData gameOptionsData)
        {
            gameOptionsData.Serialize(writer);
        }

        public static void Deserialize(IMessageReader reader, GameOptionsData gameOptionsData)
        {
            gameOptionsData.Deserialize(reader.ReadBytesAndSize());
        }
    }
}
