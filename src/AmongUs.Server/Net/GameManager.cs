using System.Collections.Concurrent;
using System.Security.Cryptography;
using AmongUs.Shared.Innersloth;

namespace AmongUs.Server.Net
{
    public class GameManager
    {
        private readonly RNGCryptoServiceProvider _random;
        private readonly ConcurrentDictionary<int, Game> _games;

        public GameManager()
        {
            _random = new RNGCryptoServiceProvider();
            _games = new ConcurrentDictionary<int, Game>();
        }
        
        public Game Create(Client owner, GameOptionsData options)
        {
            var gameCode = GameCode.GenerateCode(6);
            var game = new Game(gameCode, options);

            if (_games.TryAdd(gameCode, game))
            {
                return game;
            }

            return null;
        }

        public void Remove(int key)
        {
            _games.TryRemove(key, out _);
        }
    }
}