using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface ISwitchSystem : IShipSystem, IActivable
    {

        public byte ExpectedSwitches { get; }

        public byte ActualSwitches { get; }

        public byte Percentage { get; }

        /// <summary>
        ///     Starts the sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Start();

        /// <summary>
        ///     Stops the sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Stop();
    }
}
