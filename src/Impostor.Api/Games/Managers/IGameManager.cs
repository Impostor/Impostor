using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;

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
        /// <param name="filterOptions">Filter options.</param>
        /// <returns>Created game or null if creation was cancelled by a plugin.</returns>
        /// <exception cref="ImpostorException">Thrown when game creation failed.</exception>
        ValueTask<IGame?> CreateAsync(IGameOptions options, GameFilterOptions filterOptions);
    }
}
