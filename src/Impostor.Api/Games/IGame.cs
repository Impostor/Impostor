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

        IClientPlayer? Host { get; }

        bool IsPublic { get; }

        /// <summary>
        ///     Gets or sets display name on game list.
        /// </summary>
        string? DisplayName { get; set; }

        IDictionary<object, object> Items { get; }

        int HostId { get; }

        IClientPlayer? GetClientPlayer(int clientId);

        T? FindObjectByNetId<T>(uint netId)
            where T : IInnerNetObject;

        /// <summary>
        ///     Adds an <see cref="IPAddress" /> to the ban list of this game.
        ///     Prevents all future joins from this <see cref="IPAddress" />.
        ///     This does not kick the player with that <see cref="IPAddress" /> from the lobby.
        /// </summary>
        /// <param name="ipAddress">
        ///     The <see cref="IPAddress" /> to ban.
        /// </param>
        void BanIp(IPAddress ipAddress);

        /// <summary>
        ///     Syncs the internal <see cref="GameOptionsData" /> to all players.
        ///     Necessary to do if you modified it, otherwise it won't be used.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask SyncSettingsAsync();

        /// <summary>
        ///     Sets game's privacy.
        /// </summary>
        /// <param name="isPublic">Privacy to set.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask SetPrivacyAsync(bool isPublic);

        /// <summary>
        ///     Send the message to all players.
        /// </summary>
        /// <param name="writer">Message to send.</param>
        /// <param name="states">Required limbo state of the player.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask SendToAllAsync(IMessageWriter writer, LimboStates states = LimboStates.NotLimbo);

        /// <summary>
        ///     Send the message to all players except one.
        /// </summary>
        /// <param name="writer">Message to send.</param>
        /// <param name="senderId">The player to exclude from sending the message.</param>
        /// <param name="states">Required limbo state of the player.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask SendToAllExceptAsync(IMessageWriter writer, int senderId, LimboStates states = LimboStates.NotLimbo);

        /// <summary>
        ///     Send a message to a specific player.
        /// </summary>
        /// <param name="writer">Message to send.</param>
        /// <param name="id">ID of the client.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask SendToAsync(IMessageWriter writer, int id);
    }
}
