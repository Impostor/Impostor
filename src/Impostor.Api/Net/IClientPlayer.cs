using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net
{
    /// <summary>
    ///     Represents a player in <see cref="IGame" />.
    /// </summary>
    public interface IClientPlayer
    {
        /// <summary>
        ///     Gets the client that belongs to the player.
        /// </summary>
        IClient Client { get; }

        /// <summary>
        ///     Gets the game where the <see cref="IClientPlayer" /> belongs to.
        /// </summary>
        IGame Game { get; }

        /// <summary>
        ///     Gets or sets the current limbo state of the player.
        /// </summary>
        LimboStates Limbo { get; set; }

        IInnerPlayerControl? Character { get; }

        public bool IsHost { get; }

        /// <summary>
        ///     Checks if the specified <see cref="IInnerNetObject" /> is owned by <see cref="IClientPlayer" />.
        /// </summary>
        /// <param name="netObject">The <see cref="IInnerNetObject" />.</param>
        /// <returns>Returns true if owned by <see cref="IClientPlayer" />.</returns>
        bool IsOwner(IInnerNetObject netObject);

        ValueTask KickAsync();

        ValueTask BanAsync();
    }
}
