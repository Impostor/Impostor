using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class SkeldVent : Vent
    {
        internal SkeldVent(Ids id, Vector2 position) : base((int)id, id.ToString(), position)
        {
        }

        public enum Ids
        {
            Admin = 0,
            RightHallway = 1,
            Cafeteria = 2,
            Electrical = 3,
            UpperEngine = 4,
            Security = 5,
            Medbay = 6,
            Weapons = 7,
            LowerReactor = 8,
            LowerEngine = 9,
            Shields = 10,
            UpperReactor = 11,
            UpperNavigation = 12,
            LowerNavigation = 13,
        }
    }
}
