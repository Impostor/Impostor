using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface IReactorSystem : IShipSystem, IActivable
    {
        /// <summary>
        ///     Gets the <see cref="Time"/> left for the sabotge.
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
        /// <returns>Task that must be awaited.</returns>
        ValueTask Start(float time = 30.0f);

        /// <summary>
        ///     Stops the reactor sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Stop();
    }
}