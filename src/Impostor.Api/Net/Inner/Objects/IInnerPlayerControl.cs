using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerPlayerControl
    {
        /// <summary>
        ///     Gets the <see cref="IInnerPlayerInfo"/> of the <see cref="IInnerPlayerControl"/>.
        ///     Contains metadata about the player.
        /// </summary>
        IInnerPlayerInfo PlayerInfo { get; }

        /// <summary>
        ///     Sets the name of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="name">A name for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetNameAsync(string name);

        /// <summary>
        ///     Sets the color of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="colorId">A color for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetColorAsync(byte colorId);

        /// <summary>
        ///     Sets the hat of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="hatId">An hat for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetHatAsync(uint hatId);

        /// <summary>
        ///     Sets the pet of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="petId">An hat for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetPetAsync(uint petId);

        /// <summary>
        ///     Sets the skin of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="skinId">An hat for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetSkinAsync(uint skinId);

        /// <summary>
        ///     Send a chat message as the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SendChatAsync(string text);
    }
}
