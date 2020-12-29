using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface IReactorSystem : IShipSystem, IActivable
    {
        /// <summary>
        ///     Gets the <see cref="Countdown"/> left for the sabotge.
        /// </summary>
        float Countdown { get; }

        /// <summary>
        ///     Gets the pairs of <<see cref="IClientPlayer"/>, byte> for the consoles.
        /// </summary>
        // TODO: Change byte with an enum (?) - different for each map
        IEnumerable<Tuple<IClientPlayer, byte>> UserConsolePairs { get; }

        /// <summary>
        ///     Starts the reactor sabotage.
        /// </summary>
        /// <param name="time">Countdown for the sabotage.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Start(float time = -1f);

        /// <summary>
        ///     Stops the reactor sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Stop();
    }
}
