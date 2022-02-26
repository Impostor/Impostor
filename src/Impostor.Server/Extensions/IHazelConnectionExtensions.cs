using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Hazel;

namespace Impostor.Server
{
    public static class IHazelConnectionExtensions
    {
        /// <summary>
        /// Disconnect a connection using a custom message.
        /// </summary>
        /// <param name="connection">The connection to disconnect.</param>
        /// <param name="message">The message to disconnect with.</param>
        /// <returns>Task that should be awaited to ensure disconnection.</returns>
        public static async ValueTask CustomDisconnectAsync(this IHazelConnection connection, string message)
        {
            if (!connection.IsConnected)
            {
                return;
            }

            using var writer = MessageWriter.Get();
            MessageDisconnect.Serialize(writer, true, DisconnectReason.Custom, message);

            await connection.DisconnectAsync(DisconnectReason.Custom.ToString(), writer);
        }
    }
}
