using System;
using System.Collections.Generic;
using System.Text;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Api.Events.Ship
{
    public interface IShipOxygenStateChangedEvent : IShipEvent
    {
        public IOxygenSystem Oxygen { get; }
    }
}
