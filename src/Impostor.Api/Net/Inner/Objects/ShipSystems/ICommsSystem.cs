using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface ICommsSystem : IShipSystem, IActivable
    {
        /// <summary>
        ///     Starts the communications sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Start();

        /// <summary>
        ///     Stops the communications sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Stop();
    }
}
