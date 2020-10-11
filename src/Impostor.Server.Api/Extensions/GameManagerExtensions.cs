using System.Linq;
using Impostor.Server.Games.Managers;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server
{
    public static class GameManagerExtensions
    {
        public static int GetGameCount(this IGameManager manager, MapFlags map)
        {
            return manager.Games.Count(game => map.HasFlag((MapFlags)(1 << game.Options.MapId)));
        }
    }
}