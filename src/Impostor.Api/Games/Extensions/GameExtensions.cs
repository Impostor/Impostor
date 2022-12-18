using System;
using System.Threading.Tasks;
using Impostor.Api.Net;

namespace Impostor.Api.Games
{
    public static class GameExtensions
    {
        public static ValueTask SendToAllExceptAsync(this IGame game, IMessageWriter writer, LimboStates states, int? id)
        {
            return id.HasValue
                ? game.SendToAllExceptAsync(writer, id.Value, states)
                : game.SendToAllAsync(writer, states);
        }

        public static ValueTask SendToAllExceptAsync(this IGame game, IMessageWriter writer, LimboStates states, IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return game.SendToAllExceptAsync(writer, client.Id, states);
        }

        public static ValueTask SendToAsync(this IGame game, IMessageWriter writer, IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return game.SendToAsync(writer, client.Id);
        }

        public static ValueTask SendToAsync(this IGame game, IMessageWriter writer, IClientPlayer player)
        {
            return game.SendToAsync(writer, player.Client);
        }
    }
}
