using System.Collections.Generic;

namespace Impostor.Api.Innersloth.Maps
{
    public abstract class MapData
    {
        public static IReadOnlyDictionary<MapTypes, MapData> Maps { get; } = new Dictionary<MapTypes, MapData>
        {
            [MapTypes.Skeld] = new SkeldData(),
            [MapTypes.MiraHQ] = new MiraData(),
            [MapTypes.Polus] = new PolusData(),
            [MapTypes.Airship] = new AirshipData(),
        };

        public abstract IReadOnlyDictionary<int, Vent> Vents { get; }

        protected void ConnectVents(int vent, int? left = null, int? center = null, int? right = null)
        {
            Vents[vent].Left = left != null ? Vents[left.Value] : null;
            Vents[vent].Center = center != null ? Vents[center.Value] : null;
            Vents[vent].Right = right != null ? Vents[right.Value] : null;
        }
    }
}
