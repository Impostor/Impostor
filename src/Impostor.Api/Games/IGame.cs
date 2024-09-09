﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;

namespace Impostor.Api.Games
{
    public interface IGame
    {
        IGameOptions Options { get; }

        GameFilterOptions FilterOptions { get; }

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

        /// <summary>
        /// Gets a value indicating whether the Host of the game has requested host authority.
        /// </summary>
        /// <remarks>
        /// Vanilla Among Us does not request this, but certain client-side mods will.
        /// </remarks>
        bool IsHostAuthoritive { get; }

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
        ///     Syncs the internal <see cref="IGameOptions" /> to all players.
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

        /// <summary>
        ///     Start a GameData(To) message.
        /// </summary>
        /// <remarks>Use with caution, recommended only for advanced developers.</remarks>
        /// <param name="targetClientId">The client to target if needed, `null` if the message should be sent to all players.</param>
        /// <param name="type">Message type of the message.</param>
        /// <returns>A <see cref="IMessageWriter" /> that you can fill and send using <see cref="FinishGameDataAsync"/>.</returns>
        IMessageWriter StartGameData(int? targetClientId = null, MessageType type = MessageType.Reliable);

        /// <summary>
        ///     Finishes GameData message and sends it to either the target or all players.
        /// </summary>
        /// <remarks>Use with caution, recommended only for advanced developers.</remarks>
        /// <param name="writer">MessageWriter received from <see cref="StartGameData"/>.</param>
        /// <param name="targetClientId">Same target ClientId passed to StartGameData.</param>
        /// <returns>Task that sends the packet.</returns>
        ValueTask FinishGameDataAsync(IMessageWriter writer, int? targetClientId = null);

        /// <summary>
        ///     Creates a MessageWriter with GameData header.
        /// </summary>
        /// <remarks>Use with caution, recommended only for advanced developers.</remarks>
        /// <param name="targetNetId">Net id of the InnerNetObject.</param>
        /// <param name="callId">Rpc id of the message.</param>
        /// <param name="targetClientId">The client to target if needed, `null` if the message should be sent to all players.</param>
        /// <param name="type">Message type of the message.</param>
        /// <returns>A <see cref="IMessageWriter" /> that you can fill with rpc data and send using <see cref="FinishRpcAsync"/>.</returns>
        IMessageWriter StartRpc(uint targetNetId, RpcCalls callId, int? targetClientId = null, MessageType type = MessageType.Reliable);

        /// <summary>
        /// Finishes rpc message and sends it to either the target or all players.
        /// </summary>
        /// <remarks>Use with caution, recommended only for advanced developers.</remarks>
        /// <param name="writer">Message writer of the rpc.</param>
        /// <param name="targetClientId">Same target client id passed to <see cref="StartRpc"/>.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask FinishRpcAsync(IMessageWriter writer, int? targetClientId = null);
    }
}
