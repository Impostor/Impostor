using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface ISabotageSystem : IShipSystem
    {
        /// <summary>
        ///     Sets the cooldown of sabotage.
        /// </summary>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SetCooldown(float time);
    }
}
