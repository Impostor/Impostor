using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Events.Ship
{
    public class ShipSwitchStateChangedEvent : IShipSwitchStateChangedEvent
    {
        public ShipSwitchStateChangedEvent(IGame game, ISwitchSystem electrical)
        {
            Game = game;
            Electrical = electrical;
        }

        public IGame Game { get; }

        public ISwitchSystem Electrical { get; }

        public IInnerShipStatus ShipStatus => Game.GameNet.ShipStatus;
    }
}
