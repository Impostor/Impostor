using System;
using System.Net;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message13RedirectS2C
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

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
