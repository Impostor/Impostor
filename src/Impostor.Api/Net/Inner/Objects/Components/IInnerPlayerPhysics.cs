using System.Numerics;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.Components
{
    public interface IInnerPlayerPhysics : IInnerNetObject
    {
        /// <summary>
        ///     Enters a vent.
        ///
        ///     This will make the player walk in the direction of the target vent, and might result in it getting stuck.
        ///     Can be cancelled using <see cref="ExitVentAsync" /> or <see cref="BootFromVentAsync" />.
        /// </summary>
        /// <param name="ventId">ID of the vent.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask EnterVentAsync(int ventId);

        /// <summary>
        ///     Exits a vent.
        /// </summary>
        /// <param name="ventId">ID of the vent.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask ExitVentAsync(int ventId);

        /// <summary>
        ///     Boots the player from a vent.
        /// </summary>
        /// <param name="ventId">ID of the vent.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask BootFromVentAsync(int ventId);

        /// <summary>
        ///     Climbs a ladder.
        /// </summary>
        /// <param name="ladderId">ID of the ladder.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask ClimbLadderAsync(byte ladderId);

        /// <summary>
        ///     Starts petting the player's pet.
        ///     This will make the player walk in the direction of the target position, and may cause it to get stuck.
        /// </summary>
        /// <param name="position">Position for the player to walk to.</param>
        /// <param name="petPosition">Position for the pet to teleport to.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask PetAsync(Vector2 position, Vector2 petPosition);

        /// <summary>
        ///     Stops petting the player's pet.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask CancelPetAsync();
    }
}
