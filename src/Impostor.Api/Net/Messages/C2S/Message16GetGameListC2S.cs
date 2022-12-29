using System;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message16GetGameListC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out IGameOptions options, out QuickChatModes chatMode, out CrossplayFlags crossplayFlags, out GameFilterOptions gameFilterOptions)
        {
            var version = reader.ReadPackedInt32();
            if (version != 2)
            {
                throw new NotSupportedException($"Version {version} of {nameof(Message16GetGameListC2S)} is not supported");
            }

            options = GameOptionsFactory.Deserialize(reader);
            chatMode = (QuickChatModes)reader.ReadByte();
            crossplayFlags = (CrossplayFlags)reader.ReadInt32();
            gameFilterOptions = GameFilterOptions.Deserialize(reader);
        }
    }
}
