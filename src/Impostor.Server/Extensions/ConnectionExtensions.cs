using Hazel;
using Impostor.Server.Net.Response;

namespace Impostor.Server.Extensions
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