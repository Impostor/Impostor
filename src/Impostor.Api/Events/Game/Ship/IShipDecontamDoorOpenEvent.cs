using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Ship
{
    /// <summary>
    ///     Called whenever decontamination doors are opened on Mira HQ or Polus
    /// </summary>
    public interface IShipDecontamDoorOpenEvent : IShipEvent
    {
        /// <summary>
        /// Gets the <see cref="SystemTypes"/> of the decontamination whose doors were opened
        /// </summary>
        public SystemTypes SystemType { get; }

        /// <summary>
        /// Gets the raw byte value of the door, as seen in packets.
        /// </summary>
        public byte DoorId { get; }

        /// <summary>
        /// Gets the decontam door which was opened, and from which side.
        /// </summary>
        public DecontamDoors DecontamDoor { get; }
    }
}