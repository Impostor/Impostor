using Impostor.Api.Games;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc62CheckVanish
    {
        public static void Serialize(IMessageWriter writer, float maxDuration = 0f)
        {
            writer.Write(maxDuration);
        }

        public static void Deserialize(IMessageReader reader, out float maxDuration)
        {
            maxDuration = reader.ReadSingle();
        }
    }
}
