using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message08EndGameC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out GameOverReason gameOverReason)
        {
            gameOverReason = (GameOverReason)reader.ReadByte();
            reader.ReadBoolean(); // showAd
        }
    }
}
