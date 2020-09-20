using System.Collections.Concurrent;
using AmongUs.Shared.Innersloth;
using Serilog;

namespace AmongUs.Server.Net
{
    public class GameManager
    {
        private static readonly ILogger Logger = Log.ForContext<GameManager>();
        
        private readonly ConcurrentDictionary<int, Game> _games;

        public GameManager()
        {
            _games = new ConcurrentDictionary<int, Game>();
        }
        
        public Game Create(Client owner, GameOptionsData options)
        {
            var gameCode = GameCode.GenerateCode(6);
            var game = new Game(this, gameCode, options);

            if (_games.TryAdd(gameCode, game))
            {
                Logger.Debug("Created game with code {0} ({1}).", GameCode.IntToGameName(gameCode), gameCode);        
                return game;
            }

            Logger.Warning("Failed to create game.");
            return null;
        }

        public Game Find(int gameCode)
        {
            _games.TryGetValue(gameCode, out var game);
            return game;
        }

        public void Remove(int gameCode)
        {
            Logger.Debug("Remove game with code {0} ({1}).", GameCode.IntToGameName(gameCode), gameCode);   
            _games.TryRemove(gameCode, out _);
        }
    }
}