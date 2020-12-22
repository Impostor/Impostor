using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Events.Ship
{
    public class ShipCommsStateChangedEvent : IShipCommsStateChangedEvent
    {
        public ShipCommsStateChangedEvent(IGame game, ICommsSystem comms)
        {
            Game = game;
            Comms = comms;
        }

        public IGame Game { get; }

        public ICommsSystem Comms { get; }

        public IInnerShipStatus ShipStatus => Game.GameNet.ShipStatus;
    }
}
