using System;
using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class SkeldVent : IVent
    {
        private readonly Lazy<SkeldVent>? _left;

        private readonly Lazy<SkeldVent>? _center;

        private readonly Lazy<SkeldVent>? _right;

        internal SkeldVent(SkeldData data, Ids id, Vector2 position, Ids? left = null, Ids? center = null, Ids? right = null)
        {
            Id = id;
            Name = id.ToString();
            Position = position;

            _left = left == null ? null : new Lazy<SkeldVent>(() => data.Vents[left.Value]);
            _center = center == null ? null : new Lazy<SkeldVent>(() => data.Vents[center.Value]);
            _right = right == null ? null : new Lazy<SkeldVent>(() => data.Vents[right.Value]);
        }

        public enum Ids
        {
            Admin = 0,
            RightHallway = 1,
            Cafeteria = 2,
            Electrical = 3,
            UpperEngine = 4,
            Security = 5,
            Medbay = 6,
            Weapons = 7,
            LowerReactor = 8,
            LowerEngine = 9,
            Shields = 10,
            UpperReactor = 11,
            UpperNavigation = 12,
            LowerNavigation = 13,
        }

        public Ids Id { get; }

        int IVent.Id => (int)Id;

        public string Name { get; }

        public Vector2 Position { get; }

        public SkeldVent? Left => _left?.Value;

        IVent? IVent.Left => Left;

        public SkeldVent? Center => _center?.Value;

        IVent? IVent.Center => Center;

        public SkeldVent? Right => _right?.Value;

        IVent? IVent.Right => Right;
    }
}
