using System.Net;
using Hazel;

namespace Impostor.Server.Net.Messages
{
    internal static class Message13Redirect
    {
        public static void Serialize(MessageWriter writer, bool clear, IPEndPoint ipEndPoint)
        {
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.Redirect);
            writer.Write(ipEndPoint.Address.GetAddressBytes());
            writer.Write((ushort) ipEndPoint.Port);
            writer.EndMessage();
        }
    }
}