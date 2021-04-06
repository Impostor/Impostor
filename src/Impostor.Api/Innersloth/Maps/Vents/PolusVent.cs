using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class PolusVent : Vent
    {
        internal PolusVent(Ids id, Vector2 position) : base((int)id, id.ToString(), position)
        {
        }

        public enum Ids
        {
            Security = 0,
            Electrical = 1,
            O2 = 2,
            Communications = 3,
            Office = 4,
            Admin = 5,
            Laboratory = 6,
            Lava = 7,
            Storage = 8,
            RightStabilizer = 9,
            LeftStabilizer = 10,
            OutsideAdmin = 11,
        }
    }
}
