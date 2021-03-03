using Impostor.Api.Net.Messages;

namespace Impostor.Api.Reactor
{
    public static class ModdedHandshakeS2C
    {
        public static void Serialize(IMessageWriter writer, string serverBrand)
        {
            writer.StartMessage(byte.MaxValue);
            writer.Write(serverBrand);
            writer.EndMessage();
        }
    }
}
