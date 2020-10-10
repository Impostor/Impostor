using System.Threading.Tasks;
using Impostor.Server.Games;

namespace Impostor.Server.Net.Messages
{
    /// <summary>
    ///     Represents the message writer for <see cref="IGame"/>.
    /// </summary>
    public interface IGameMessageWriter : IMessageWriter
    {
        /// <summary>
        ///     Send the message to all players.
        /// </summary>
        /// <param name="states">Required limbo state of the player.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask SendToAllAsync(LimboStates states = LimboStates.NotLimbo);

        /// <summary>
        ///     Send the message to all players except one.
        /// </summary>
        /// <param name="senderId">The player to exclude from sending the message.</param>
        /// <param name="states">Required limbo state of the player.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask SendToAllExceptAsync(int senderId, LimboStates states = LimboStates.NotLimbo);

        /// <summary>
        ///     Send a message to a specific player.
        /// </summary>
        /// <param name="id">ID of the client.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask SendToAsync(int id);
    }
}