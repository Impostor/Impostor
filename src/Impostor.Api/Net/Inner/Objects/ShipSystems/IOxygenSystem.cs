using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface IOxygenSystem : IShipSystem, IActivable
    {
        /// <summary>
        ///     Gets the <see cref="Countdown"/> left for the sabotge.
        /// </summary>
        float Countdown { get; }

        IEnumerable<int> CompletedConsoles { get; }

        /// <summary>
        ///     Starts the reactor sabotage.
        /// </summary>
        /// <param name="time">Countdown for the sabotage.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Start(float time = 30.0f);

        /// <summary>
        ///     Stops the reactor sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Stop();
    }
}
