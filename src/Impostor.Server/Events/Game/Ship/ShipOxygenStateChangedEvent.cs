using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Events.Ship
{
    public class ShipOxygenStateChangedEvent : IShipOxygenStateChangedEvent
    {
        public ShipOxygenStateChangedEvent(IGame game, IOxygenSystem oxygen)
        {
            Game = game;
            Oxygen = oxygen;
        }

        public IGame Game { get; }

        public IOxygenSystem Oxygen { get; }

        public IInnerShipStatus ShipStatus => Game.GameNet.ShipStatus;
    }
}
