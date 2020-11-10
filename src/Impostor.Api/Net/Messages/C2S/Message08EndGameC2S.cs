using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message08EndGameC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out GameOverReason gameOverReason, out bool showAd)
        {
            gameOverReason = (GameOverReason)reader.ReadByte();
            showAd = reader.ReadBoolean(); // Perhaps we could disable ads on Impostor?
        }
    }
}