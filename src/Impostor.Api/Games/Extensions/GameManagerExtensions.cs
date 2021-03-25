using System.Linq;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Games
{
    public static class GameManagerExtensions
    {
        public static int GetGameCount(this IGameManager manager, MapFlags map)
        {
            return manager.Games.Count(game => map.HasFlag((MapFlags)(1 << (byte)game.Options.Map)));
        }
    }
}
