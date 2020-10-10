using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server
{
    public interface IGameManager
    {
        IEnumerable<IGame> Games { get; }

        ValueTask<IGame> CreateAsync(GameOptionsData options);

        IGame? Find(GameCode code);

        IEnumerable<IGame> FindListings(MapFlags map, int impostorCount, GameKeywords language, int count = 10);

        ValueTask RemoveAsync(GameCode code);
    }
}