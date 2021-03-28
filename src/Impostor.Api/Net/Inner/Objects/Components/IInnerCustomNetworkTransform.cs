using System.Numerics;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.Components
{
    public interface IInnerCustomNetworkTransform : IInnerNetObject
    {
        /// <summary>
        ///     Gets position where the object thinks it is (not interpolated).
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        ///     Gets current object's velocity.
        /// </summary>
        Vector2 Velocity { get; }

        /// <summary>
        ///     Snaps the current to the given position <see cref="IInnerPlayerControl" />.
        /// </summary>
        /// <param name="position">The target position.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SnapToAsync(Vector2 position);
    }
}
