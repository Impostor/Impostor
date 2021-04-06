using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class AirshipData : MapData
    {
        public AirshipData()
        {
            Vents = new[]
            {
                new AirshipVent(AirshipVent.Ids.Vault, new Vector2(-12.6322f, 8.4735f)),
                new AirshipVent(AirshipVent.Ids.Cockpit, new Vector2(-22.099f, -1.512f)),
                new AirshipVent(AirshipVent.Ids.ViewingDeck, new Vector2(-15.659f, -11.6991f)),
                new AirshipVent(AirshipVent.Ids.EngineRoom, new Vector2(0.203f, -2.5361f)),
                new AirshipVent(AirshipVent.Ids.Kitchen, new Vector2(-2.6019f, -9.338f)),
                new AirshipVent(AirshipVent.Ids.MainHallBottom, new Vector2(7.021f, -3.730999f)),
                new AirshipVent(AirshipVent.Ids.GapRight, new Vector2(9.814f, 3.206f)),
                new AirshipVent(AirshipVent.Ids.GapLeft, new Vector2(12.663f, 5.922f)),
                new AirshipVent(AirshipVent.Ids.MainHallTop, new Vector2(3.605f, 6.923f)),
                new AirshipVent(AirshipVent.Ids.Showers, new Vector2(23.9869f, -1.386f)),
                new AirshipVent(AirshipVent.Ids.Records, new Vector2(23.2799f, 8.259998f)),
                new AirshipVent(AirshipVent.Ids.CargoBay, new Vector2(30.4409f, -3.577f)),
            }.ToDictionary(x => x.Id, x => (Vent)x);

            ConnectVents(AirshipVent.Ids.Vault, left: AirshipVent.Ids.Cockpit);
            ConnectVents(AirshipVent.Ids.Cockpit, left: AirshipVent.Ids.Vault, right: AirshipVent.Ids.ViewingDeck);
            ConnectVents(AirshipVent.Ids.ViewingDeck, left: AirshipVent.Ids.Cockpit);
            ConnectVents(AirshipVent.Ids.EngineRoom, left: AirshipVent.Ids.Kitchen, right: AirshipVent.Ids.MainHallBottom);
            ConnectVents(AirshipVent.Ids.Kitchen, left: AirshipVent.Ids.EngineRoom, right: AirshipVent.Ids.MainHallBottom);
            ConnectVents(AirshipVent.Ids.MainHallBottom, left: AirshipVent.Ids.EngineRoom, right: AirshipVent.Ids.Kitchen);
            ConnectVents(AirshipVent.Ids.GapRight, left: AirshipVent.Ids.MainHallTop, right: AirshipVent.Ids.GapLeft);
            ConnectVents(AirshipVent.Ids.GapLeft, left: AirshipVent.Ids.MainHallTop, right: AirshipVent.Ids.GapRight);
            ConnectVents(AirshipVent.Ids.MainHallTop, left: AirshipVent.Ids.GapLeft, right: AirshipVent.Ids.GapRight);
            ConnectVents(AirshipVent.Ids.Showers, left: AirshipVent.Ids.Records, right: AirshipVent.Ids.CargoBay);
            ConnectVents(AirshipVent.Ids.Records, left: AirshipVent.Ids.Showers, right: AirshipVent.Ids.CargoBay);
            ConnectVents(AirshipVent.Ids.CargoBay, left: AirshipVent.Ids.Showers, right: AirshipVent.Ids.Records);
        }

        public override IReadOnlyDictionary<int, Vent> Vents { get; }

        private void ConnectVents(AirshipVent.Ids vent, AirshipVent.Ids? left = null, AirshipVent.Ids? center = null, AirshipVent.Ids? right = null)
        {
            ConnectVents((int)vent, (int?)left, (int?)center, (int?)right);
        }
    }
}
