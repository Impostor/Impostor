using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Games.Managers
{
    public interface IGameManager
    {
        IEnumerable<IGame> Games { get; }

        IGame? Find(GameCode code);

        /// <summary>
        /// Creates a new game.
        /// </summary>
        /// <param name="options">Game options.</param>
        /// <returns>Created game or null if creation was cancelled by a plugin.</returns>
        /// <exception cref="ImpostorException">Thrown when game creation failed.</exception>
        ValueTask<IGame?> CreateAsync(GameOptionsData options);
    }
}
