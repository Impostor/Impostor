using System.Collections.Generic;
using System.Linq;
using Impostor.Server.Games;
using Impostor.Server.Games.Managers;

// ReSharper disable once CheckNamespace
namespace Impostor.Server.Net.Manager
{
    internal partial class GameManager : IGameManager
    {
        IEnumerable<IGame> IGameManager.Games => _games.Select(kv => kv.Value);

        IGame IGameManager.Find(GameCode code) => Find(code);
    }
}