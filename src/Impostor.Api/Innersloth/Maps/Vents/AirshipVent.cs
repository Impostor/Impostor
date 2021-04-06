using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class AirshipVent : Vent
    {
        internal AirshipVent(Ids id, Vector2 position) : base((int)id, id.ToString(), position)
        {
        }

        public enum Ids
        {
            Vault = 0,
            Cockpit = 1,
            ViewingDeck = 2,
            EngineRoom = 3,
            Kitchen = 4,
            MainHallBottom = 5,
            GapRight = 6,
            GapLeft = 7,
            MainHallTop = 8,
            Showers = 9,
            Records = 10,
            CargoBay = 11,
        }
    }
}
