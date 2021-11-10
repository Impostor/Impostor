using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message16GetGameListC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out GameOptionsData options, out QuickChatModes chatMode)
        {
            var version = reader.ReadPackedInt32();
            if (version != 2)
            {
                throw new NotSupportedException($"Version {version} of {nameof(Message16GetGameListC2S)} is not supported");
            }

            options = GameOptionsData.DeserializeCreate(reader);
            chatMode = (QuickChatModes)reader.ReadByte();
            reader.ReadInt32(); // crossplayFlags, not used yet
        }
    }
}
