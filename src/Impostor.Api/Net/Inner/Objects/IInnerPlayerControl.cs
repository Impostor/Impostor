using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Inner.Objects.Components;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerPlayerControl : IInnerNetObject
    {
        /// <summary>
        ///     Gets the <see cref="PlayerId" /> assigned by the client of the host of the game.
        /// </summary>
        byte PlayerId { get; }

        /// <summary>
        ///     Gets the <see cref="IInnerPlayerPhysics" /> of the <see cref="IInnerPlayerControl" />.
        ///     Contains vent logic.
        /// </summary>
        IInnerPlayerPhysics Physics { get; }

        /// <summary>
        ///     Gets the <see cref="IInnerCustomNetworkTransform" /> of the <see cref="IInnerPlayerControl" />.
        ///     Contains position data about the player.
        /// </summary>
        IInnerCustomNetworkTransform NetworkTransform { get; }

        /// <summary>
        ///     Gets the <see cref="IInnerPlayerInfo" /> of the <see cref="IInnerPlayerControl" />.
        ///     Contains metadata about the player.
        /// </summary>
        IInnerPlayerInfo PlayerInfo { get; }

        /// <summary>
        ///     Sets the name of the current <see cref="IInnerPlayerControl" />.
        ///     Visible to all players.
        /// </summary>
        /// <param name="name">A name for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetNameAsync(string name);

        /// <summary>
        ///     Sets the color of the current <see cref="IInnerPlayerControl" />.
        ///     Visible to all players.
        /// </summary>
        /// <param name="colorType">A color for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetColorAsync(ColorType colorType);

        /// <summary>
        ///     Sets the hat of the current <see cref="IInnerPlayerControl" />.
        ///     Visible to all players.
        /// </summary>
        /// <param name="hatId">An hat for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetHatAsync(string hatId);

        /// <summary>
        ///     Sets the pet of the current <see cref="IInnerPlayerControl" />.
        ///     Visible to all players.
        /// </summary>
        /// <param name="petId">A pet for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetPetAsync(string petId);

        /// <summary>
        ///     Sets the skin of the current <see cref="IInnerPlayerControl" />.
        ///     Visible to all players.
        /// </summary>
        /// <param name="skinId">A skin for the player.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetSkinAsync(string skinId);

        /// <summary>
        ///     Send a chat message as the current <see cref="IInnerPlayerControl" />.
        ///     Visible to all players.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SendChatAsync(string text);

        /// <summary>
        ///     Send a chat message as the current <see cref="IInnerPlayerControl" />.
        ///     Visible to only the current.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="player">
        ///     The player that should receive this chat message.
        ///     When left as null, will send message to self.
        /// </param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SendChatToPlayerAsync(string text, IInnerPlayerControl? player = null);

        /// <summary>
        ///     Murder <paramref name="target" /> player or remove their protective shield.
        /// </summary>
        /// <param name="target">Target player to murder.</param>
        /// <param name="result">The result of the murder operation.</param>
        /// <exception cref="ImpostorProtocolException">Thrown when player is not the impostor.</exception>
        /// <exception cref="ImpostorProtocolException">Thrown when player is dead.</exception>
        /// <exception cref="ImpostorProtocolException">Thrown when target is dead.</exception>
        /// <returns>Task that must be awaited.</returns>
        ValueTask MurderPlayerAsync(IInnerPlayerControl target, MurderResultFlags result);

        /// <summary>
        ///     Murder <paramref name="target" /> player successfully.
        /// </summary>
        /// <param name="target">Target player to murder.</param>
        /// <exception cref="ImpostorProtocolException">Thrown when player is not the impostor.</exception>
        /// <exception cref="ImpostorProtocolException">Thrown when player is dead.</exception>
        /// <exception cref="ImpostorProtocolException">Thrown when target is dead.</exception>
        /// <returns>Task that must be awaited.</returns>
        ValueTask MurderPlayerAsync(IInnerPlayerControl target);

        /// <summary>
        ///     Protect <paramref name="target" /> player.
        /// </summary>
        /// <param name="target">Target player to protect.</param>
        /// <exception cref="ImpostorProtocolException">Thrown when target is a guardian angel.</exception>
        /// <returns>Task that must be awaited.</returns>
        ValueTask ProtectPlayerAsync(IInnerPlayerControl target);

        /// <summary>
        ///     Exile the current player. This doesn't produce a body to be reported.
        ///     Visible to all players.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask ExileAsync();
    }
}
