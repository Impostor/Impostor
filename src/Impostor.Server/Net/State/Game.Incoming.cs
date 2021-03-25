using System;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Server.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        private readonly SemaphoreSlim _clientAddLock = new SemaphoreSlim(1, 1);

        public async ValueTask HandleStartGame(IMessageReader message)
        {
            GameState = GameStates.Starting;

            using var packet = MessageWriter.Get(MessageType.Reliable);
            message.CopyTo(packet);
            await SendToAllAsync(packet);

            await _eventManager.CallAsync(new GameStartingEvent(this));
        }

        public async ValueTask HandleEndGame(IMessageReader message, GameOverReason gameOverReason)
        {
            GameState = GameStates.Ended;

            // Broadcast end of the game.
            using (var packet = MessageWriter.Get(MessageType.Reliable))
            {
                message.CopyTo(packet);
                await SendToAllAsync(packet);
            }

            // Put all players in the correct limbo state.
            foreach (var player in _players)
            {
                player.Value.Limbo = LimboStates.PreSpawn;
            }

            await _eventManager.CallAsync(new GameEndedEvent(this, gameOverReason));
        }

        public async ValueTask HandleAlterGame(IMessageReader message, IClientPlayer sender, bool isPublic)
        {
            IsPublic = isPublic;

            using var packet = MessageWriter.Get(MessageType.Reliable);
            message.CopyTo(packet);
            await SendToAllExceptAsync(packet, sender.Client.Id);

            await _eventManager.CallAsync(new GameAlterEvent(this, isPublic));
        }

        public async ValueTask HandleRemovePlayer(int playerId, DisconnectReason reason)
        {
            await PlayerRemove(playerId);

            // It's possible that the last player was removed, so check if the game is still around.
            if (GameState == GameStates.Destroyed)
            {
                return;
            }

            using var packet = MessageWriter.Get(MessageType.Reliable);
            WriteRemovePlayerMessage(packet, false, playerId, reason);
            await SendToAllExceptAsync(packet, playerId);
        }

        public async ValueTask HandleKickPlayer(int playerId, bool isBan)
        {
            _logger.LogInformation("{0} - Player {1} has left.", Code, playerId);

            using var message = MessageWriter.Get(MessageType.Reliable);

            // Send message to everyone that this player was kicked.
            WriteKickPlayerMessage(message, false, playerId, isBan);

            await SendToAllAsync(message);
            await PlayerRemove(playerId, isBan);

            // Remove the player from everyone's game.
            WriteRemovePlayerMessage(
                message,
                true,
                playerId,
                isBan ? DisconnectReason.Banned : DisconnectReason.Kicked);

            await SendToAllExceptAsync(message, playerId);
        }

        public async ValueTask<GameJoinResult> AddClientAsync(ClientBase client)
        {
            var hasLock = false;

            try
            {
                hasLock = await _clientAddLock.WaitAsync(TimeSpan.FromMinutes(1));

                if (hasLock)
                {
                    return await AddClientSafeAsync(client);
                }
            }
            finally
            {
                if (hasLock)
                {
                    _clientAddLock.Release();
                }
            }

            return GameJoinResult.FromError(GameJoinError.InvalidClient);
        }

        private async ValueTask HandleJoinGameNew(ClientPlayer sender, bool isNew)
        {
            _logger.LogInformation("{0} - Player {1} ({2}) is joining.", Code, sender.Client.Name, sender.Client.Id);

            // Add player to the game.
            if (isNew)
            {
                await PlayerAdd(sender);
            }

            sender.InitializeSpawnTimeout();

            using (var message = MessageWriter.Get(MessageType.Reliable))
            {
                WriteJoinedGameMessage(message, false, sender);
                WriteAlterGameMessage(message, false, IsPublic);

                sender.Limbo = LimboStates.NotLimbo;

                await SendToAsync(message, sender.Client.Id);
                await BroadcastJoinMessage(message, true, sender);
            }
        }

        private async ValueTask<GameJoinResult> AddClientSafeAsync(ClientBase client)
        {
            // Check if the IP of the player is banned.
            if (_bannedIps.Contains(client.Connection.EndPoint.Address))
            {
                return GameJoinResult.FromError(GameJoinError.Banned);
            }

            var player = client.Player;

            // Check if;
            // - The player is already in this game.
            // - The game is full.
            if (player?.Game != this && _players.Count >= Options.MaxPlayers)
            {
                return GameJoinResult.FromError(GameJoinError.GameFull);
            }

            if (GameState == GameStates.Starting || GameState == GameStates.Started)
            {
                return GameJoinResult.FromError(GameJoinError.GameStarted);
            }

            if (GameState == GameStates.Destroyed)
            {
                return GameJoinResult.FromError(GameJoinError.GameDestroyed);
            }

            var isNew = false;

            if (player == null || player.Game != this)
            {
                var clientPlayer = new ClientPlayer(_serviceProvider.GetRequiredService<ILogger<ClientPlayer>>(), client, this);

                if (!_clientManager.Validate(client))
                {
                    return GameJoinResult.FromError(GameJoinError.InvalidClient);
                }

                isNew = true;
                player = clientPlayer;
                client.Player = clientPlayer;
            }

            // Check current player state.
            if (player.Limbo == LimboStates.NotLimbo)
            {
                return GameJoinResult.FromError(GameJoinError.InvalidLimbo);
            }

            if (GameState == GameStates.Ended)
            {
                await HandleJoinGameNext(player, isNew);
                return GameJoinResult.CreateSuccess(player);
            }

            await HandleJoinGameNew(player, isNew);
            return GameJoinResult.CreateSuccess(player);
        }

        private async ValueTask HandleJoinGameNext(ClientPlayer sender, bool isNew)
        {
            _logger.LogInformation("{0} - Player {1} ({2}) is rejoining.", Code, sender.Client.Name, sender.Client.Id);

            // Add player to the game.
            if (isNew)
            {
                await PlayerAdd(sender);
            }

            // Check if the host joined and let everyone join.
            if (sender.Client.Id == HostId)
            {
                GameState = GameStates.NotStarted;

                // Spawn the host.
                await HandleJoinGameNew(sender, false);

                // Pull players out of limbo.
                await CheckLimboPlayers();
                return;
            }

            sender.Limbo = LimboStates.WaitingForHost;

            using (var packet = MessageWriter.Get(MessageType.Reliable))
            {
                WriteWaitForHostMessage(packet, false, sender);

                await SendToAsync(packet, sender.Client.Id);
                await BroadcastJoinMessage(packet, true, sender);
            }
        }
    }
}
