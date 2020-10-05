using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Impostor.Server.Data;
using Impostor.Server.Net.Redirector;
using Impostor.Server.Net.State;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Impostor.Server.Net.Manager
{
    internal class GameManager
    {
        private readonly ILogger<GameManager> _logger;
        private readonly INodeLocator _nodeLocator;
        private readonly IPEndPoint _publicIp;
        private readonly ConcurrentDictionary<int, Game> _games;

        public GameManager(ILogger<GameManager> logger, IOptions<ServerConfig> config, INodeLocator nodeLocator)
        {
            _logger = logger;
            _nodeLocator = nodeLocator;
            _publicIp = new IPEndPoint(IPAddress.Parse(config.Value.PublicIp), config.Value.PublicPort);
            _games = new ConcurrentDictionary<int, Game>();
        }
        
        public Game Create(GameOptionsData options)
        {
            // TODO: Prevent duplicates when using server redirector using INodeProvider.
            
            var gameCode = GameCode.GenerateCode(6);
            var gameCodeStr = GameCode.IntToGameName(gameCode);
            var game = new Game(this, _nodeLocator, _publicIp, gameCode, options);

            if (_nodeLocator.Find(gameCodeStr) == null && 
                _games.TryAdd(gameCode, game))
            {
                _nodeLocator.Save(gameCodeStr, _publicIp);
                _logger.LogDebug("Created game with code {0} ({1}).", game.CodeStr, gameCode);        
                return game;
            }

            _logger.LogWarning("Failed to create game.");
            return null;
        }

        public Game Find(int gameCode)
        {
            _games.TryGetValue(gameCode, out var game);
            return game;
        }

        public IEnumerable<Game> FindListings(MapFlags map, int impostorCount, GameKeywords language, int count = 10)
        {
            var results = 0;
            
            // Find games that have not started yet.
            foreach (var (code, game) in _games.Where(x => 
                x.Value.IsPublic &&
                x.Value.GameState == GameStates.NotStarted && 
                x.Value.PlayerCount < x.Value.Options.MaxPlayers))
            {
                // Check for options.
                if (!map.HasFlag((MapFlags) (1 << game.Options.MapId)))
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

        public int GetGameCount(MapFlags map)
        {
            var count = 0;

            foreach (var (code, game) in _games) {
                if (!map.HasFlag((MapFlags)(1 << game.Options.MapId)))
                {
                    continue;
                }
                count++;
            }
            return count;
        }

        public void Remove(int gameCode)
        {
            _logger.LogDebug("Remove game with code {0} ({1}).", GameCode.IntToGameName(gameCode), gameCode);
            _nodeLocator.Remove(GameCode.IntToGameName(gameCode));
            _games.TryRemove(gameCode, out _);
        }
    }
}