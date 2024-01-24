using System.Collections.Generic;
using System.Numerics;

namespace Impostor.Api.Innersloth.Maps;

public abstract class MapData
{
    protected internal MapData()
    {
    }

    public static IReadOnlyDictionary<MapTypes, MapData> Maps { get; } = new Dictionary<MapTypes, MapData>
    {
        [MapTypes.Skeld] = new SkeldData(),
        [MapTypes.MiraHQ] = new MiraData(),
        [MapTypes.Polus] = new PolusData(),
        [MapTypes.Dleks] = new AprilData(),
        [MapTypes.Airship] = new AirshipData(),
        [MapTypes.Fungle] = new FungleData(),
    }.AsReadOnly();

    public abstract IReadOnlyDictionary<int, VentData> Vents { get; }

    public abstract IReadOnlyDictionary<int, TaskData> Tasks { get; }

    public abstract IReadOnlyDictionary<int, DoorData> Doors { get; }

    public abstract float SpawnRadius { get; }

    public abstract Vector2 InitialSpawnCenter { get; }

    public abstract Vector2 MeetingSpawnCenter { get; }

    public abstract Vector2 MeetingSpawnCenter2 { get; }
}
