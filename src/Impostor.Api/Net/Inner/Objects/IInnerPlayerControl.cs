using System.Threading.Tasks;
using Impostor.Api.Innersloth.Customization;

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

        /// <param name="colorType">A color for the player.</param>
        /// <inheritdoc cref="SetColorAsync(byte)" />
        ValueTask SetColorAsync(ColorType colorType);

        /// <summary>
        ///     Sets the hat of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="hatId">An hat for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetHatAsync(uint hatId);

        /// <param name="hatType">An hat for the player.</param>
        /// <inheritdoc cref="SetHatAsync(uint)" />
        ValueTask SetHatAsync(HatType hatType);

        /// <summary>
        ///     Sets the pet of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="petId">A pet for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetPetAsync(uint petId);

        /// <param name="petType">A pet for the player.</param>
        /// <inheritdoc cref="SetPetAsync(uint)" />
        ValueTask SetPetAsync(PetType petType);

        /// <summary>
        ///     Sets the skin of the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="skinId">A skin for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetSkinAsync(uint skinId);

        /// <param name="skinType">A skin for the player.</param>
        /// <inheritdoc cref="SetSkinAsync(uint)" />
        ValueTask SetSkinAsync(SkinType skinType);

        /// <summary>
        ///     Send a chat message as the current <see cref="IInnerPlayerControl"/>.
        ///     Visible to all players.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SendChatAsync(string text);
    }
}
