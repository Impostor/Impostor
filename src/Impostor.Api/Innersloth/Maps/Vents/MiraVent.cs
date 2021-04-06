using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class MiraVent : Vent
    {
        internal MiraVent(Ids id, Vector2 position) : base((int)id, id.ToString(), position)
        {
        }

        public enum Ids
        {
            Balcony = 1,
            Cafeteria = 2,
            Reactor = 3,
            Laboratory = 4,
            Office = 5,
            Admin = 6,
            Greenhouse = 7,
            Medbay = 8,
            Decontamination = 9,
            LockerRoom = 10,
            Launchpad = 11,
        }
    }
}
