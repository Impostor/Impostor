using System.Numerics;

namespace Impostor.Api.Innersloth.Maps;

public sealed class DoorData
{
    internal DoorData(int id, SystemTypes room, Vector2 position)
    {
        Id = id;
        Room = room;
        Position = position;
    }

    public int Id { get; }

    public SystemTypes Room { get; }

    public Vector2 Position { get; }
}
