using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Ship
{
    public class ShipDoorsCloseEvent : IShipDoorsCloseEvent
    {
        public ShipDoorsCloseEvent(IGame game, IInnerShipStatus shipStatus, IClientPlayer clientPlayer, SystemTypes systemType)
        {
            Game = game;
            ShipStatus = shipStatus;
            ClientPlayer = clientPlayer;
            SystemType = systemType;
        }
        
        public IGame Game { get; }
        
        public IInnerShipStatus ShipStatus { get; }

        public IClientPlayer ClientPlayer { get; }
        
        public SystemTypes SystemType { get; }
    }
}