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
        /// <param name="reason">The reason to disconnect with.</param>
        /// <param name="message">The custom message to disconnect with if <paramref name="reason"/> is <see cref="DisconnectReason.Custom"/>.</param>
        /// <returns>Task that should be awaited to ensure disconnection.</returns>
        public static async ValueTask CustomDisconnectAsync(this IHazelConnection connection, DisconnectReason reason, string? message = null)
        {
            if (!connection.IsConnected)
            {
                return;
            }

            using var writer = MessageWriter.Get();
            MessageDisconnect.Serialize(writer, true, reason, message);

            await connection.DisconnectAsync(reason.ToString(), writer);
        }
    }
}
