using System;
using System.Numerics;

namespace Impostor.Api.Innersloth.Maps;

public class VentData
{
    private readonly Lazy<VentData>? _left;

    private readonly Lazy<VentData>? _center;

    private readonly Lazy<VentData>? _right;

    internal VentData(MapData data, int id, string name, Vector2 position, int? left = null, int? center = null, int? right = null)
    {
        Id = id;
        Name = name;
        Position = position;

        _left = left == null ? null : new Lazy<VentData>(() => data.Vents[left.Value]);
        _center = center == null ? null : new Lazy<VentData>(() => data.Vents[center.Value]);
        _right = right == null ? null : new Lazy<VentData>(() => data.Vents[right.Value]);
    }

    public int Id { get; }

    public string Name { get; }

    public Vector2 Position { get; }

    public VentData? Left => _left?.Value;

    public VentData? Center => _center?.Value;

    public VentData? Right => _right?.Value;
}
