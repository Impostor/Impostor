using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class MiraData : MapData
    {
        public MiraData()
        {
            Vents = new[]
            {
                new MiraVent(MiraVent.Ids.Balcony, new Vector2(23.77f, -1.94f)),
                new MiraVent(MiraVent.Ids.Cafeteria, new Vector2(23.9f, 7.18f)),
                new MiraVent(MiraVent.Ids.Reactor, new Vector2(0.4800001f, 10.697f)),
                new MiraVent(MiraVent.Ids.Laboratory, new Vector2(11.606f, 13.816f)),
                new MiraVent(MiraVent.Ids.Office, new Vector2(13.28f, 20.13f)),
                new MiraVent(MiraVent.Ids.Admin, new Vector2(22.39f, 17.23f)),
                new MiraVent(MiraVent.Ids.Greenhouse, new Vector2(17.85f, 25.23f)),
                new MiraVent(MiraVent.Ids.Medbay, new Vector2(15.41f, -1.82f)),
                new MiraVent(MiraVent.Ids.Decontamination, new Vector2(6.83f, 3.145f)),
                new MiraVent(MiraVent.Ids.LockerRoom, new Vector2(4.29f, 0.5299997f)),
                new MiraVent(MiraVent.Ids.Launchpad, new Vector2(-6.18f, 3.56f)),
            }.ToDictionary(x => x.Id, x => (Vent)x);

            ConnectVents(MiraVent.Ids.Balcony, left: MiraVent.Ids.Medbay, right: MiraVent.Ids.Cafeteria);
            ConnectVents(MiraVent.Ids.Cafeteria, left: MiraVent.Ids.Admin, right: MiraVent.Ids.Balcony);
            ConnectVents(MiraVent.Ids.Reactor, left: MiraVent.Ids.Laboratory, center: MiraVent.Ids.Decontamination, right: MiraVent.Ids.Launchpad);
            ConnectVents(MiraVent.Ids.Laboratory, left: MiraVent.Ids.Reactor, center: MiraVent.Ids.Decontamination, right: MiraVent.Ids.Office);
            ConnectVents(MiraVent.Ids.Office, left: MiraVent.Ids.Laboratory, center: MiraVent.Ids.Admin, right: MiraVent.Ids.Greenhouse);
            ConnectVents(MiraVent.Ids.Admin, left: MiraVent.Ids.Greenhouse, center: MiraVent.Ids.Cafeteria, right: MiraVent.Ids.Office);
            ConnectVents(MiraVent.Ids.Greenhouse, left: MiraVent.Ids.Admin, right: MiraVent.Ids.Office);
            ConnectVents(MiraVent.Ids.Medbay, left: MiraVent.Ids.Balcony, right: MiraVent.Ids.LockerRoom);
            ConnectVents(MiraVent.Ids.Decontamination, left: MiraVent.Ids.Reactor, center: MiraVent.Ids.LockerRoom, right: MiraVent.Ids.Laboratory);
            ConnectVents(MiraVent.Ids.LockerRoom, left: MiraVent.Ids.Medbay, center: MiraVent.Ids.Launchpad, right: MiraVent.Ids.Decontamination);
            ConnectVents(MiraVent.Ids.Launchpad, left: MiraVent.Ids.Reactor, right: MiraVent.Ids.LockerRoom);
        }

        public override IReadOnlyDictionary<int, Vent> Vents { get; }

        private void ConnectVents(MiraVent.Ids vent, MiraVent.Ids? left = null, MiraVent.Ids? center = null, MiraVent.Ids? right = null)
        {
            ConnectVents((int)vent, (int?)left, (int?)center, (int?)right);
        }
    }
}
