using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerPlayerInfo
    {
        /// <summary>
        ///     Gets the name of the player as decided by the host.
        /// </summary>
        string PlayerName { get; }

        /// <summary>
        ///     Gets the color of the player.
        /// </summary>
        byte ColorId { get; }

        /// <summary>
        ///     Gets the hat of the player.
        /// </summary>
        uint HatId { get; }

        /// <summary>
        ///     Gets the pet of the player.
        /// </summary>
        uint PetId { get; }

        /// <summary>
        ///     Gets the skin of the player.
        /// </summary>
        uint SkinId { get; }

        /// <summary>
        ///     Gets a value indicating whether the player is an impostor.
        /// </summary>
        bool IsImpostor { get; }

        /// <summary>
        ///     Gets a value indicating whether the player is a dead in the current game.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        ///     Gets the reason why the player is dead in the current game.
        /// </summary>
        DeathReason LastDeathReason { get; }
    }
}