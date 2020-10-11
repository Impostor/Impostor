using System.Threading.Tasks;
using Impostor.Server.Games;
using Impostor.Server.Net.Manager;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net
{
    /// <summary>
    ///     Represents a player in <see cref="IGame"/>.
    /// </summary>
    public interface IClientPlayer
    {
        /// <summary>
        ///     Gets the client that belongs to the player.
        /// </summary>
        IClient Client { get; }

        /// <summary>
        ///     Gets the game where the <see cref="IClientPlayer"/> belongs to.
        /// </summary>
        IGame Game { get; }

        /// <summary>
        ///     Gets or sets the current limbo state of the player.
        /// </summary>
        LimboStates Limbo { get; set; }

        ValueTask KickAsync();

        ValueTask BanAsync();
    }
}