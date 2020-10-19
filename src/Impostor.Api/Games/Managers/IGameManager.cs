using System.Collections.Generic;

namespace Impostor.Server.Games.Managers
{
    public interface IGameManager
    {
        IEnumerable<IGame> Games { get; }

        IGame? Find(GameCode code);
    }
}