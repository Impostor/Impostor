using Impostor.Api.Events.Ship;

namespace Impostor.Api.Innersloth
{
    public enum SystemTypes : byte
    {
        Hallway,
        Storage,
        Cafeteria,
        Reactor,
        UpperEngine,
        Nav,
        Admin,
        Electrical,
        LifeSupp,
        Shields,
        MedBay,
        Security,
        Weapons,
        LowerEngine,
        Comms,
        ShipTasks,
        Doors,
        Sabotage,
        Decontamination,
        Launchpad,
        LockerRoom,
        Laboratory,
        Balcony,
        Office,
        Greenhouse,
        /// <summary>
        /// This enum value does not exist in the original enum. This is only used for <see cref="IShipDecontamDoorOpenEvent"/>
        /// </summary>
        TopDecontaminationPolus = 26, // Should we keep this here so modders know about it?
    }
}