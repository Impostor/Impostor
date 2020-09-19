using AmongUs.Server.Net.Response;
using Hazel;

namespace AmongUs.Server.Extensions
{
    internal static class ConnectionExtensions
    {
        public static void Send(this Connection connection, MessageBase message)
        {
            using (message)
            {
                connection.Send(message.Write());
            }
        }
    }
}