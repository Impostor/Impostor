using System.Collections.Generic;

namespace Impostor.Api.Games.Managers
{
    public interface IGameManager
    {
        IEnumerable<IGame> Games { get; }

        IGame? Find(GameCode code);
    }
}