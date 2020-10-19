using System;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Net
{
    public static class GameMessageWriterExtensions
    {
        public static ValueTask SendToAllExceptAsync(this IGameMessageWriter writer, LimboStates states, int? id)
        {
            return id.HasValue
                ? writer.SendToAllExceptAsync(id.Value, states)
                : writer.SendToAllAsync(states);
        }

        public static ValueTask SendToAllExceptAsync(this IGameMessageWriter writer, LimboStates states, IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return writer.SendToAllExceptAsync(client.Id, states);
        }

        public static ValueTask SendToAsync(this IGameMessageWriter writer, IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return writer.SendToAsync(client.Id);
        }

        public static ValueTask SendToAsync(this IGameMessageWriter writer, IClientPlayer player)
        {
            return SendToAsync(writer, player.Client);
        }
    }
}