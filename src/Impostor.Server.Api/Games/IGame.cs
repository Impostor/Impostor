using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Impostor.Server.Net;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Games
{
    public interface IGame
    {
        GameOptionsData Options { get; }

        GameCode Code { get; }

        GameStates GameState { get; }

        IEnumerable<IClientPlayer> Players { get; }

        IPEndPoint PublicIp { get; }

        int PlayerCount { get; }

        IClientPlayer Host { get; }

        bool IsPublic { get; }

        IDictionary<object, object> Items { get; }

        int HostId { get; }

        IGameMessageWriter CreateMessage(MessageType type);

        bool TryGetPlayer(int id, [NotNullWhen(true)] out IClientPlayer player);

        /// <summary>
        ///     Register a new client to the game.
        /// </summary>
        /// <param name="client">Client to register.</param>
        /// <returns>Join result.</returns>
        ValueTask<GameJoinResult> AddClientAsync(IClient client);

        /// <summary>
        ///     Kicks all the players from the game to end the game.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask EndAsync();

        ValueTask HandleStartGame(IMessageReader reader);

        ValueTask HandleEndGame(IMessageReader reader);

        ValueTask HandleKickPlayer(int playerId, bool isBan);

        ValueTask HandleRemovePlayer(int playerId, DisconnectReason reason);

        ValueTask HandleAlterGame(IMessageReader message, IClientPlayer sender, bool isPublic);
    }
}