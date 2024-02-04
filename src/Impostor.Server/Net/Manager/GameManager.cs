﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Manager;
using Impostor.Server.Events;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Manager;

internal class GameManager(
    ILogger<GameManager> logger,
    IOptions<ServerConfig> config,
    IServiceProvider serviceProvider,
    IEventManager eventManager,
    IGameCodeFactory gameCodeFactory,
    IOptions<CompatibilityConfig> compatibilityConfig,
    ICompatibilityManager compatibilityManager)
    : IGameManager
{
    private readonly CompatibilityConfig _compatibilityConfig = compatibilityConfig.Value;
    private readonly ICompatibilityManager _compatibilityManager = compatibilityManager;
    private readonly ConcurrentDictionary<int, Game> _games = new();

    private readonly IPEndPoint _publicIp = new(IPAddress.Parse(config.Value.ResolvePublicIp()),
        config.Value.PublicPort);

    IEnumerable<IGame> IGameManager.Games => _games.Select(kv => kv.Value);

    IGame? IGameManager.Find(GameCode code)
    {
        return Find(code);
    }

    public ValueTask<IGame?> CreateAsync(IGameOptions options, GameFilterOptions filterOptions)
    {
        return CreateAsync(null, options, filterOptions);
    }

    public Game? Find(GameCode code)
    {
        _games.TryGetValue(code, out var game);
        return game;
    }

    public async ValueTask RemoveAsync(GameCode gameCode)
    {
        if (_games.TryGetValue(gameCode, out var game) && game.PlayerCount > 0)
        {
            foreach (var player in game.Players)
            {
                await player.KickAsync();
            }

            return;
        }

        if (!_games.TryRemove(gameCode, out game))
        {
            return;
        }

        logger.LogDebug("Remove game with code {0} ({1}).", GameCodeParser.IntToGameName(gameCode), gameCode);

        await eventManager.CallAsync(new GameDestroyedEvent(game));
    }

    public async ValueTask<IGame?> CreateAsync(IClient? owner, IGameOptions options, GameFilterOptions filterOptions)
    {
        var @event = new GameCreationEvent(this, owner);
        await eventManager.CallAsync(@event);

        if (@event.IsCancelled)
        {
            return null;
        }

        var (success, game) = await TryCreateAsync(options, filterOptions, @event.GameCode);

        for (var i = 0; i < 10 && !success; i++)
        {
            (success, game) = await TryCreateAsync(options, filterOptions);
        }

        if (!success || game == null)
        {
            throw new ImpostorException("Could not create new game"); // TODO: Fix generic exception.
        }

        return game;
    }

    private async ValueTask<(bool Success, Game? Game)> TryCreateAsync(IGameOptions options,
        GameFilterOptions filterOptions, GameCode? desiredGameCode = null)
    {
        var gameCode = desiredGameCode ?? gameCodeFactory.Create();
        var game = ActivatorUtilities.CreateInstance<Game>(serviceProvider, _publicIp, gameCode, options,
            filterOptions);

        if (!_games.TryAdd(gameCode, game))
        {
            return (false, null);
        }

        logger.LogDebug("Created game with code {0}.", game.Code);

        await eventManager.CallAsync(new GameCreatedEvent(game));

        return (true, game);
    }
}
