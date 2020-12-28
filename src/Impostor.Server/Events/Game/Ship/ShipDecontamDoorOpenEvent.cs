using Impostor.Api.Events.Ship;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Events.Ship
{
    public class ShipDecontamDoorOpenEvent : IShipDecontamDoorOpenEvent
    {
        public ShipDecontamDoorOpenEvent(IGame game, IInnerShipStatus shipStatus, IClientPlayer clientPlayer, SystemTypes systemType, byte doorId)
        {
            Game = game;
            ShipStatus = shipStatus;
            ClientPlayer = clientPlayer;
            SystemType = systemType;
            DoorId = doorId;
            DecontamDoor = DecontamDoorsParser.Parse(game.Options.MapId, systemType, doorId);
        }
        
        public IGame Game { get; }
        
        public IInnerShipStatus ShipStatus { get; }

        public IClientPlayer ClientPlayer { get; }

        public SystemTypes SystemType { get; }
        
        public byte DoorId { get; }
        
        public DecontamDoors DecontamDoor { get; }
    }
}