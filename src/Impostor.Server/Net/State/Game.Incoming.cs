using System;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Hazel;
using Impostor.Server.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.State;

internal partial class Game
{
    private readonly SemaphoreSlim _clientAddLock = new(1, 1);

    public async ValueTask HandleStartGameAsync(IMessageReader message)
    {
        GameState = GameStates.Starting;

        using var packet = MessageWriter.Get(MessageType.Reliable);
        message.CopyTo(packet);
        await SendToAllAsync(packet);

        await eventManager.CallAsync(new GameStartingEvent(this));
    }

    public async ValueTask HandleEndGameAsync(IMessageReader message, GameOverReason gameOverReason)
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

        await eventManager.CallAsync(new GameEndedEvent(this, gameOverReason));
    }

    public async ValueTask HandleAlterGameAsync(IMessageReader message, IClientPlayer sender, bool isPublic)
    {
        IsPublic = isPublic;

        using var packet = MessageWriter.Get(MessageType.Reliable);
        message.CopyTo(packet);
        await SendToAllExceptAsync(packet, sender.Client.Id);

        await eventManager.CallAsync(new GameAlterEvent(this, isPublic));
    }

    public async ValueTask HandleRemovePlayerAsync(int playerId, DisconnectReason reason)
    {
        await PlayerRemoveAsync(playerId);

        // It's possible that the last player was removed, so check if the game is still around.
        if (GameState == GameStates.Destroyed)
        {
            return;
        }

        using var packet = MessageWriter.Get(MessageType.Reliable);
        WriteRemovePlayerMessage(packet, false, playerId, reason);
        await SendToAllExceptAsync(packet, playerId);
    }

    public async ValueTask HandleKickPlayerAsync(int playerId, bool isBan)
    {
        logger.LogInformation("{0} - Player {1} has left.", Code, playerId);

        using var message = MessageWriter.Get(MessageType.Reliable);

        // Send message to everyone that this player was kicked.
        WriteKickPlayerMessage(message, false, playerId, isBan);

        await SendToAllAsync(message);
        await PlayerRemoveAsync(playerId, isBan);

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

    private async ValueTask HandleJoinGameNewAsync(ClientPlayer sender, bool isNew)
    {
        logger.LogInformation("{0} - Player {1} ({2}) is joining.", Code, sender.Client.Name, sender.Client.Id);

        // Add player to the game.
        if (isNew)
        {
            await PlayerAddAsync(sender);
        }

        sender.InitializeSpawnTimeout();

        using var message = MessageWriter.Get(MessageType.Reliable);
        WriteJoinedGameMessage(message, false, sender);
        WriteAlterGameMessage(message, false, IsPublic);

        sender.Limbo = LimboStates.NotLimbo;

        await SendToAsync(message, sender.Client.Id);
        await BroadcastJoinMessage(message, true, sender);
    }

    private async ValueTask<GameJoinResult> AddClientSafeAsync(ClientBase client)
    {
        // Check if the IP of the player is banned.
        if (_bannedIps.Contains(client.Connection.EndPoint.Address))
        {
            return GameJoinResult.FromError(GameJoinError.Banned);
        }

        var player = client.Player;

        // Check if the player is running the same version as the host
        if (_compatibilityConfig.AllowVersionMixing == false &&
            Host != null && client.GameVersion != Host.Client.GameVersion)
        {
            var versionCheckResult = compatibilityManager.CanJoinGame(Host.Client.GameVersion, client.GameVersion);
            if (versionCheckResult != GameJoinError.None)
            {
                return GameJoinResult.FromError(versionCheckResult);
            }
        }

        switch (GameState)
        {
            case GameStates.Starting:
            case GameStates.Started:
                return GameJoinResult.FromError(GameJoinError.GameStarted);
            case GameStates.Destroyed:
                return GameJoinResult.FromError(GameJoinError.GameDestroyed);
        }

        // Check if;
        // - The player is already in this game.
        // - The game is full.
        if (player?.Game != this && _players.Count >= Options.MaxPlayers)
        {
            return GameJoinResult.FromError(GameJoinError.GameFull);
        }

        var isNew = false;

        if (player == null || player.Game != this)
        {
            var clientPlayer = new ClientPlayer(serviceProvider.GetRequiredService<ILogger<ClientPlayer>>(), client,
                this, _timeoutConfig.SpawnTimeout);

            if (!clientManager.Validate(client))
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
            await HandleJoinGameNextAsync(player, isNew);
            return GameJoinResult.CreateSuccess(player);
        }

        var @event = new GamePlayerJoiningEvent(this, player);
        await eventManager.CallAsync(@event);

        if (@event.JoinResult is { IsSuccess: false })
        {
            return @event.JoinResult.Value;
        }

        await HandleJoinGameNewAsync(player, isNew);
        return GameJoinResult.CreateSuccess(player);
    }

    private async ValueTask HandleJoinGameNextAsync(ClientPlayer sender, bool isNew)
    {
        logger.LogInformation("{0} - Player {1} ({2}) is rejoining.", Code, sender.Client.Name, sender.Client.Id);

        // Add player to the game.
        if (isNew)
        {
            await PlayerAddAsync(sender);
        }

        // Check if the host joined and let everyone join.
        if (sender.Client.Id == HostId)
        {
            GameState = GameStates.NotStarted;

            // Spawn the host.
            await HandleJoinGameNewAsync(sender, false);

            // Pull players out of limbo.
            await CheckLimboPlayersAsync();
            return;
        }

        sender.Limbo = LimboStates.WaitingForHost;

        using var packet = MessageWriter.Get(MessageType.Reliable);
        WriteWaitForHostMessage(packet, false, sender);

        await SendToAsync(packet, sender.Client.Id);
        await BroadcastJoinMessage(packet, true, sender);
    }
}
