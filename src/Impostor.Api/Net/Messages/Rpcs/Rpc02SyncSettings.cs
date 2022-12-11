using Impostor.Api.Innersloth.GameOptions;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc02SyncSettings
    {
        public static void Serialize(IMessageWriter writer, IGameOptions gameOptionsData)
        {
            GameOptionsFactory.Serialize(writer, gameOptionsData);
        }

        public static void Deserialize(IMessageReader reader, out IGameOptions gameOptionsData)
        {
            gameOptionsData = GameOptionsFactory.Deserialize(reader);
        }

        public static void DeserializeInto(IMessageReader reader, IGameOptions gameOptionsData)
        {
            GameOptionsFactory.DeserializeInto(reader, gameOptionsData);
        }
    }
}
