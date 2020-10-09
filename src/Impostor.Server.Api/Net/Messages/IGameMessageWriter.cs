using System.Threading.Tasks;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net
{
    public interface IGameMessageWriter : IMessageWriter
    {
        /// <summary>
        ///     Send the message to all players.
        /// </summary>
        /// <param name="states"></param>
        ValueTask SendToAllAsync(LimboStates states);

        /// <summary>
        ///     Send the message to all players except one.
        /// </summary>
        /// <param name="states"></param>
        /// <param name="senderId">The player to exclude from sending the message.</param>
        ValueTask SendToAllExceptAsync(LimboStates states, int senderId);
        
        /// <summary>
        ///     Send a message to a specific player.
        /// </summary>
        /// <param name="id"></param>
        ValueTask SendToAsync(int id);
    }
}