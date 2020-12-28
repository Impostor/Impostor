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
        /// <param name="startingPosition">Initial state of the switches. (bit mask)</param>
        /// <param name="expectedPosition">Expected state of the switches. (bit mask)</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Start(byte startingPosition = 0, byte expectedPosition = 0b11111);

        /// <summary>
        ///     Stops the sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask Stop();
    }
}
