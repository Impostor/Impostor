using System.Collections.Generic;

namespace Impostor.Api.Innersloth.Maps
{
    public interface IMapData
    {
        public static IReadOnlyDictionary<MapTypes, IMapData> Maps { get; } = new Dictionary<MapTypes, IMapData>
        {
            [MapTypes.Skeld] = new SkeldData(),
            [MapTypes.MiraHQ] = new MiraData(),
            [MapTypes.Polus] = new PolusData(),
            [MapTypes.Airship] = new AirshipData(),
        }.AsReadOnly();

        IReadOnlyDictionary<int, IVent> Vents { get; }
    }
}
