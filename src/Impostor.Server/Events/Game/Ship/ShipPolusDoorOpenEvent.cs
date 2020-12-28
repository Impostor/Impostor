using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Ship
{
    public class ShipPolusDoorOpenEvent : IShipPolusDoorOpenEvent
    {
        public ShipPolusDoorOpenEvent(IGame game, IInnerShipStatus shipStatus, IClientPlayer clientPlayer, PolusDoors door)
        {
            Game = game;
            ShipStatus = shipStatus;
            ClientPlayer = clientPlayer;
            Door = door;
        }
        
        public IGame Game { get; }
        
        public IInnerShipStatus ShipStatus { get; }

        public IClientPlayer ClientPlayer { get; }
        
        public PolusDoors Door { get; }
    }
}