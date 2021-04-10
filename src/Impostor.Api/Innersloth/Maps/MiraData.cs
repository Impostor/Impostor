using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class MiraData : IMapData
    {
        private readonly IReadOnlyDictionary<int, IVent> _vents;

        public MiraData()
        {
            var vents = new[]
            {
                new MiraVent(this, MiraVent.Ids.Balcony, new Vector2(23.77f, -1.94f), left: MiraVent.Ids.Medbay, right: MiraVent.Ids.Cafeteria),
                new MiraVent(this, MiraVent.Ids.Cafeteria, new Vector2(23.9f, 7.18f), left: MiraVent.Ids.Admin, right: MiraVent.Ids.Balcony),
                new MiraVent(this, MiraVent.Ids.Reactor, new Vector2(0.4800001f, 10.697f), left: MiraVent.Ids.Laboratory, center: MiraVent.Ids.Decontamination, right: MiraVent.Ids.Launchpad),
                new MiraVent(this, MiraVent.Ids.Laboratory, new Vector2(11.606f, 13.816f), left: MiraVent.Ids.Reactor, center: MiraVent.Ids.Decontamination, right: MiraVent.Ids.Office),
                new MiraVent(this, MiraVent.Ids.Office, new Vector2(13.28f, 20.13f), left: MiraVent.Ids.Laboratory, center: MiraVent.Ids.Admin, right: MiraVent.Ids.Greenhouse),
                new MiraVent(this, MiraVent.Ids.Admin, new Vector2(22.39f, 17.23f), left: MiraVent.Ids.Greenhouse, center: MiraVent.Ids.Cafeteria, right: MiraVent.Ids.Office),
                new MiraVent(this, MiraVent.Ids.Greenhouse, new Vector2(17.85f, 25.23f), left: MiraVent.Ids.Admin, right: MiraVent.Ids.Office),
                new MiraVent(this, MiraVent.Ids.Medbay, new Vector2(15.41f, -1.82f), left: MiraVent.Ids.Balcony, right: MiraVent.Ids.LockerRoom),
                new MiraVent(this, MiraVent.Ids.Decontamination, new Vector2(6.83f, 3.145f), left: MiraVent.Ids.Reactor, center: MiraVent.Ids.LockerRoom, right: MiraVent.Ids.Laboratory),
                new MiraVent(this, MiraVent.Ids.LockerRoom, new Vector2(4.29f, 0.5299997f), left: MiraVent.Ids.Medbay, center: MiraVent.Ids.Launchpad, right: MiraVent.Ids.Decontamination),
                new MiraVent(this, MiraVent.Ids.Launchpad, new Vector2(-6.18f, 3.56f), left: MiraVent.Ids.Reactor, right: MiraVent.Ids.LockerRoom),
            };

            Vents = vents.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _vents = vents.ToDictionary(x => (int)x.Id, x => (IVent)x).AsReadOnly();
        }

        public IReadOnlyDictionary<MiraVent.Ids, MiraVent> Vents { get; }

        IReadOnlyDictionary<int, IVent> IMapData.Vents => _vents;
    }
}
