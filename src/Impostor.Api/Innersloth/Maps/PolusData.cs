using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class PolusData : IMapData
    {
        private readonly IReadOnlyDictionary<int, IVent> _vents;

        public PolusData()
        {
            var vents = new[]
            {
                new PolusVent(this, PolusVent.Ids.Security, new Vector2(1.929f, -9.558001f), left: PolusVent.Ids.O2, right: PolusVent.Ids.Electrical),
                new PolusVent(this, PolusVent.Ids.Electrical, new Vector2(6.9f, -14.41f), left: PolusVent.Ids.O2, right: PolusVent.Ids.Security),
                new PolusVent(this, PolusVent.Ids.O2, new Vector2(3.51f, -16.58f), left: PolusVent.Ids.Electrical, right: PolusVent.Ids.Security),
                new PolusVent(this, PolusVent.Ids.Communications, new Vector2(12.304f, -18.898f), left: PolusVent.Ids.Storage, right: PolusVent.Ids.Office),
                new PolusVent(this, PolusVent.Ids.Office, new Vector2(16.379f, -19.599f), left: PolusVent.Ids.Communications, right: PolusVent.Ids.Storage),
                new PolusVent(this, PolusVent.Ids.Admin, new Vector2(20.089f, -25.517f), left: PolusVent.Ids.OutsideAdmin, right: PolusVent.Ids.Lava),
                new PolusVent(this, PolusVent.Ids.Laboratory, new Vector2(32.963f, -9.526f), right: PolusVent.Ids.Lava),
                new PolusVent(this, PolusVent.Ids.Lava, new Vector2(30.907f, -11.86f), left: PolusVent.Ids.Laboratory, right: PolusVent.Ids.Admin),
                new PolusVent(this, PolusVent.Ids.Storage, new Vector2(22f, -12.19f), left: PolusVent.Ids.Communications, right: PolusVent.Ids.Office),
                new PolusVent(this, PolusVent.Ids.RightStabilizer, new Vector2(24.02f, -8.39f), left: PolusVent.Ids.LeftStabilizer),
                new PolusVent(this, PolusVent.Ids.LeftStabilizer, new Vector2(9.64f, -7.72f), left: PolusVent.Ids.RightStabilizer),
                new PolusVent(this, PolusVent.Ids.OutsideAdmin, new Vector2(18.93f, -24.85f), right: PolusVent.Ids.Admin),
            };

            Vents = vents.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _vents = vents.ToDictionary(x => (int)x.Id, x => (IVent)x).AsReadOnly();
        }

        public IReadOnlyDictionary<PolusVent.Ids, PolusVent> Vents { get; }

        IReadOnlyDictionary<int, IVent> IMapData.Vents => _vents;
    }
}
