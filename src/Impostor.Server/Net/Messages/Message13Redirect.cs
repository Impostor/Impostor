using System.Net;

namespace Impostor.Server.Net.Messages
{
    internal static class Message13Redirect
    {
        public static void Serialize(IMessageWriter writer, bool clear, IPEndPoint ipEndPoint)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.Redirect);
            writer.Write(ipEndPoint.Address);
            writer.Write((ushort)ipEndPoint.Port);
            writer.EndMessage();
        }
    }
}