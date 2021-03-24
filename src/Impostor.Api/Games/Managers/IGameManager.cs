using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Games.Managers
{
    public interface IGameManager
    {
        IEnumerable<IGame> Games { get; }

        IGame? Find(GameCode code);

        ValueTask<IGame> CreateAsync(GameOptionsData options);
    }
}
