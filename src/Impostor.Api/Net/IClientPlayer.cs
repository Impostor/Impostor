using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net;
using Impostor.Api.Innersloth.Net.Objects;

namespace Impostor.Api.Net
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

        InnerPlayerControl Character { get; }

        public bool IsHost { get; }

        /// <summary>
        ///     Checks if the specified <see cref="InnerNetObject"/> is owned by <see cref="IClientPlayer"/>.
        /// </summary>
        /// <param name="netObject">The <see cref="InnerNetObject"/>.</param>
        /// <returns>Returns true if owned by <see cref="IClientPlayer"/>.</returns>
        bool IsOwner(InnerNetObject netObject);

        ValueTask KickAsync();

        ValueTask BanAsync();
    }
}