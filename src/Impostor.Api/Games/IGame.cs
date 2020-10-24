using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Games
{
    public interface IGame
    {
        GameOptionsData Options { get; }

        GameCode Code { get; }

        GameStates GameState { get; }

        IGameNet GameNet { get; }

        IEnumerable<IClientPlayer> Players { get; }

        IPEndPoint PublicIp { get; }

        int PlayerCount { get; }

        IClientPlayer Host { get; }

        bool IsPublic { get; }

        IDictionary<object, object> Items { get; }

        int HostId { get; }

        IClientPlayer GetClientPlayer(int clientId);

        /// <summary>
        ///     Send the message to all players.
        /// </summary>
        /// <param name="writer">Message to send.</param>
        /// <param name="states">Required limbo state of the player.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask SendToAllAsync(IMessageWriter writer, LimboStates states = LimboStates.NotLimbo);

        /// <summary>
        ///     Send the message to all players except one.
        /// </summary>
        /// <param name="writer">Message to send.</param>
        /// <param name="senderId">The player to exclude from sending the message.</param>
        /// <param name="states">Required limbo state of the player.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask SendToAllExceptAsync(IMessageWriter writer, int senderId, LimboStates states = LimboStates.NotLimbo);

        /// <summary>
        ///     Send a message to a specific player.
        /// </summary>
        /// <param name="writer">Message to send.</param>
        /// <param name="id">ID of the client.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask SendToAsync(IMessageWriter writer, int id);
    }
}