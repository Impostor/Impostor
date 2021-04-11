using System;
using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class PolusVent : IVent
    {
        private readonly Lazy<PolusVent>? _left;

        private readonly Lazy<PolusVent>? _center;

        private readonly Lazy<PolusVent>? _right;

        internal PolusVent(PolusData data, Ids id, Vector2 position, Ids? left = null, Ids? center = null, Ids? right = null)
        {
            Id = id;
            Name = id.ToString();
            Position = position;

            _left = left == null ? null : new Lazy<PolusVent>(() => data.Vents[left.Value]);
            _center = center == null ? null : new Lazy<PolusVent>(() => data.Vents[center.Value]);
            _right = right == null ? null : new Lazy<PolusVent>(() => data.Vents[right.Value]);
        }

        public enum Ids
        {
            Security = 0,
            Electrical = 1,
            O2 = 2,
            Communications = 3,
            Office = 4,
            Admin = 5,
            Laboratory = 6,
            Lava = 7,
            Storage = 8,
            RightStabilizer = 9,
            LeftStabilizer = 10,
            OutsideAdmin = 11,
        }

        public Ids Id { get; }

        int IVent.Id => (int)Id;

        public string Name { get; }

        public Vector2 Position { get; }

        public PolusVent? Left => _left?.Value;

        IVent? IVent.Left => Left;

        public PolusVent? Center => _center?.Value;

        IVent? IVent.Center => Center;

        public PolusVent? Right => _right?.Value;

        IVent? IVent.Right => Right;
    }
}
