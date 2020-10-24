using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Data;
using Impostor.Server.Config;
using Impostor.Server.Extensions;
using Impostor.Server.Net.Redirector;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Manager
{
    internal class GameManager : IGameManager
    {
        private readonly ILogger<GameManager> _logger;
        private readonly INodeLocator _nodeLocator;
        private readonly IPEndPoint _publicIp;
        private readonly ConcurrentDictionary<int, Game> _games;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventManager _eventManager;
        private readonly IGameCodeFactory _gameCodeFactory;

        public GameManager(ILogger<GameManager> logger, IOptions<ServerConfig> config, INodeLocator nodeLocator, IServiceProvider serviceProvider, IEventManager eventManager, IGameCodeFactory gameCodeFactory)
        {
            _logger = logger;
            _nodeLocator = nodeLocator;
            _serviceProvider = serviceProvider;
            _eventManager = eventManager;
            _gameCodeFactory = gameCodeFactory;
            _publicIp = new IPEndPoint(IPAddress.Parse(config.Value.PublicIp), config.Value.PublicPort);
            _games = new ConcurrentDictionary<int, Game>();
        }

        IEnumerable<IGame> IGameManager.Games => _games.Select(kv => kv.Value);

        IGame IGameManager.Find(GameCode code) => Find(code);

        public async ValueTask<IGame> CreateAsync(GameOptionsData options)
        {
            // TODO: Prevent duplicates when using server redirector using INodeProvider.
            var (success, game) = await TryCreateAsync(options);

            for (int i = 0; i < 10 && !success; i++)
            {
                (success, game) = await TryCreateAsync(options);
            }

            if (!success)
            {
                throw new ImpostorException("Could not create new game"); // TODO: Fix generic exception.
            }

            return game;
        }

        private async ValueTask<(bool success, Game game)> TryCreateAsync(GameOptionsData options)
        {
            var gameCode = _gameCodeFactory.Create();
            var gameCodeStr = gameCode.Code;
            var game = ActivatorUtilities.CreateInstance<Game>(_serviceProvider, _publicIp, gameCode, options);

            if (await _nodeLocator.ExistsAsync(gameCodeStr) || !_games.TryAdd(gameCode, game))
            {
                return (false, null);
            }

            await _nodeLocator.SaveAsync(gameCodeStr, _publicIp);
            _logger.LogDebug("Created game with code {0}.", game.Code);

            await _eventManager.CallAsync(new GameCreatedEvent(game));

            return (true, game);
        }

        public Game Find(GameCode code)
        {
            _games.TryGetValue(code, out var game);
            return game;
        }

        public IEnumerable<Game> FindListings(MapFlags map, int impostorCount, GameKeywords language, int count = 10)
        {
            var results = 0;

            // Find games that have not started yet.
            foreach (var (_, game) in _games.Where(x =>
                x.Value.IsPublic &&
                x.Value.GameState == GameStates.NotStarted &&
                x.Value.PlayerCount < x.Value.Options.MaxPlayers))
            {
                // Check for options.
                if (!map.HasFlag((MapFlags)(1 << game.Options.MapId)))
                {
                    continue;
                }

                if (!language.HasFlag(game.Options.Keywords))
                {
                    continue;
                }

                if (impostorCount != 0 && game.Options.NumImpostors != impostorCount)
                {
                    continue;
                }

                // Add to result.
                yield return game;

                // Break out if we have enough.
                if (++results == count)
                {
                    yield break;
                }
            }
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

            _logger.LogDebug("Remove game with code {0} ({1}).", GameCodeParser.IntToGameName(gameCode), gameCode);
            await _nodeLocator.RemoveAsync(GameCodeParser.IntToGameName(gameCode));

            await _eventManager.CallAsync(new GameDestroyedEvent(game));
        }
    }
}