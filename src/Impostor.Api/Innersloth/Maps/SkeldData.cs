using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class SkeldData : MapData
    {
        public SkeldData()
        {
            Vents = new[]
            {
                new SkeldVent(SkeldVent.Ids.Admin, new Vector2(2.544f, -9.955201f)),
                new SkeldVent(SkeldVent.Ids.RightHallway, new Vector2(9.384f, -6.438f)),
                new SkeldVent(SkeldVent.Ids.Cafeteria, new Vector2(4.2588f, -0.276f)),
                new SkeldVent(SkeldVent.Ids.Electrical, new Vector2(-9.7764f, -8.034f)),
                new SkeldVent(SkeldVent.Ids.UpperEngine, new Vector2(-15.288f, 2.52f)),
                new SkeldVent(SkeldVent.Ids.Security, new Vector2(-12.534f, -6.9492f)),
                new SkeldVent(SkeldVent.Ids.Medbay, new Vector2(-10.608f, -4.176f)),
                new SkeldVent(SkeldVent.Ids.Weapons, new Vector2(8.820001f, 3.324f)),
                new SkeldVent(SkeldVent.Ids.LowerReactor, new Vector2(-20.796f, -6.9528f)),
                new SkeldVent(SkeldVent.Ids.LowerEngine, new Vector2(-15.2508f, -13.656f)),
                new SkeldVent(SkeldVent.Ids.Shields, new Vector2(9.5232f, -14.3376f)),
                new SkeldVent(SkeldVent.Ids.UpperReactor, new Vector2(-21.876f, -3.0516f)),
                new SkeldVent(SkeldVent.Ids.UpperNavigation, new Vector2(16.008f, -3.168f)),
                new SkeldVent(SkeldVent.Ids.LowerNavigation, new Vector2(16.008f, -6.384f)),
            }.ToDictionary(x => x.Id, x => (Vent)x);

            ConnectVents(SkeldVent.Ids.Admin, left: SkeldVent.Ids.Cafeteria, right: SkeldVent.Ids.RightHallway);
            ConnectVents(SkeldVent.Ids.RightHallway, left: SkeldVent.Ids.Admin, right: SkeldVent.Ids.Cafeteria);
            ConnectVents(SkeldVent.Ids.Cafeteria, left: SkeldVent.Ids.Admin, right: SkeldVent.Ids.RightHallway);
            ConnectVents(SkeldVent.Ids.Electrical, left: SkeldVent.Ids.Security, right: SkeldVent.Ids.Medbay);
            ConnectVents(SkeldVent.Ids.UpperEngine, left: SkeldVent.Ids.UpperReactor);
            ConnectVents(SkeldVent.Ids.Security, left: SkeldVent.Ids.Medbay, right: SkeldVent.Ids.Electrical);
            ConnectVents(SkeldVent.Ids.Medbay, left: SkeldVent.Ids.Security, right: SkeldVent.Ids.Electrical);
            ConnectVents(SkeldVent.Ids.Weapons, right: SkeldVent.Ids.UpperNavigation);
            ConnectVents(SkeldVent.Ids.LowerReactor, left: SkeldVent.Ids.LowerEngine);
            ConnectVents(SkeldVent.Ids.LowerEngine, left: SkeldVent.Ids.LowerReactor);
            ConnectVents(SkeldVent.Ids.Shields, left: SkeldVent.Ids.LowerNavigation);
            ConnectVents(SkeldVent.Ids.UpperReactor, left: SkeldVent.Ids.UpperEngine);
            ConnectVents(SkeldVent.Ids.UpperNavigation, right: SkeldVent.Ids.Weapons);
            ConnectVents(SkeldVent.Ids.LowerNavigation, right: SkeldVent.Ids.Shields);
        }

        public override IReadOnlyDictionary<int, Vent> Vents { get; }

        private void ConnectVents(SkeldVent.Ids vent, SkeldVent.Ids? left = null, SkeldVent.Ids? center = null, SkeldVent.Ids? right = null)
        {
            ConnectVents((int)vent, (int?)left, (int?)center, (int?)right);
        }
    }
}
