using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class Message00HostGameC2S
    {
        public static void Serialize(IMessageWriter writer, IGameOptions gameOptions, CrossplayFlags crossplayFlags, GameFilterOptions gameFilterOptions)
        {
            writer.StartMessage(MessageFlags.HostGame);
            GameOptionsFactory.Serialize(writer, gameOptions);
            writer.Write((int)crossplayFlags);
            gameFilterOptions.Serialize(writer);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out IGameOptions gameOptions, out CrossplayFlags crossplayFlags, out GameFilterOptions gameFilterOptions)
        {
            gameOptions = GameOptionsFactory.Deserialize(reader);
            crossplayFlags = (CrossplayFlags)reader.ReadInt32();
            gameFilterOptions = GameFilterOptions.Deserialize(reader);
        }
    }
}
