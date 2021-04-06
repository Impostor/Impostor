using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class PolusData : MapData
    {
        public PolusData()
        {
            Vents = new[]
            {
                new PolusVent(PolusVent.Ids.Security, new Vector2(1.929f, -9.558001f)),
                new PolusVent(PolusVent.Ids.Electrical, new Vector2(6.9f, -14.41f)),
                new PolusVent(PolusVent.Ids.O2, new Vector2(3.51f, -16.58f)),
                new PolusVent(PolusVent.Ids.Communications, new Vector2(12.304f, -18.898f)),
                new PolusVent(PolusVent.Ids.Office, new Vector2(16.379f, -19.599f)),
                new PolusVent(PolusVent.Ids.Admin, new Vector2(20.089f, -25.517f)),
                new PolusVent(PolusVent.Ids.Laboratory, new Vector2(32.963f, -9.526f)),
                new PolusVent(PolusVent.Ids.Lava, new Vector2(30.907f, -11.86f)),
                new PolusVent(PolusVent.Ids.Storage, new Vector2(22f, -12.19f)),
                new PolusVent(PolusVent.Ids.RightStabilizer, new Vector2(24.02f, -8.39f)),
                new PolusVent(PolusVent.Ids.LeftStabilizer, new Vector2(9.64f, -7.72f)),
                new PolusVent(PolusVent.Ids.OutsideAdmin, new Vector2(18.93f, -24.85f)),
            }.ToDictionary(x => x.Id, x => (Vent)x);

            ConnectVents(PolusVent.Ids.Security, left: PolusVent.Ids.O2, right: PolusVent.Ids.Electrical);
            ConnectVents(PolusVent.Ids.Electrical, left: PolusVent.Ids.O2, right: PolusVent.Ids.Security);
            ConnectVents(PolusVent.Ids.O2, left: PolusVent.Ids.Electrical, right: PolusVent.Ids.Security);
            ConnectVents(PolusVent.Ids.Communications, left: PolusVent.Ids.Storage, right: PolusVent.Ids.Office);
            ConnectVents(PolusVent.Ids.Office, left: PolusVent.Ids.Communications, right: PolusVent.Ids.Storage);
            ConnectVents(PolusVent.Ids.Admin, left: PolusVent.Ids.OutsideAdmin, right: PolusVent.Ids.Lava);
            ConnectVents(PolusVent.Ids.Laboratory, right: PolusVent.Ids.Lava);
            ConnectVents(PolusVent.Ids.Lava, left: PolusVent.Ids.Laboratory, right: PolusVent.Ids.Admin);
            ConnectVents(PolusVent.Ids.Storage, left: PolusVent.Ids.Communications, right: PolusVent.Ids.Office);
            ConnectVents(PolusVent.Ids.RightStabilizer, left: PolusVent.Ids.LeftStabilizer);
            ConnectVents(PolusVent.Ids.LeftStabilizer, left: PolusVent.Ids.RightStabilizer);
            ConnectVents(PolusVent.Ids.OutsideAdmin, right: PolusVent.Ids.Admin);
        }

        public override IReadOnlyDictionary<int, Vent> Vents { get; }

        private void ConnectVents(PolusVent.Ids vent, PolusVent.Ids? left = null, PolusVent.Ids? center = null, PolusVent.Ids? right = null)
        {
            ConnectVents((int)vent, (int?)left, (int?)center, (int?)right);
        }
    }
}
