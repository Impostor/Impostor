using System.Net;
using System.Threading.Tasks;
using Impostor.Server.Games;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Net.Manager
{
    /// <summary>
    ///     Represents the matchmaker which will listen for incoming connections.
    /// </summary>
    public interface IMatchmaker
    {
        /// <summary>
        ///     Starts the matchmaker on the given endpoint.
        /// </summary>
        /// <param name="ipEndPoint">Endpoint where the matchmaker should listen to.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask StartAsync(IPEndPoint ipEndPoint);

        /// <summary>
        ///     Stop the matchmaker.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask StopAsync();

        /// <summary>
        ///     Create a message writer that can be send to players in the game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>Message writer for the given game.</returns>
        IGameMessageWriter CreateGameMessageWriter(IGame game, MessageType messageType);
    }
}