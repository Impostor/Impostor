using System;
using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class MiraVent : IVent
    {
        private readonly Lazy<MiraVent>? _left;

        private readonly Lazy<MiraVent>? _center;

        private readonly Lazy<MiraVent>? _right;

        internal MiraVent(MiraData data, Ids id, Vector2 position, Ids? left = null, Ids? center = null, Ids? right = null)
        {
            Id = id;
            Name = id.ToString();
            Position = position;

            _left = left == null ? null : new Lazy<MiraVent>(() => data.Vents[left.Value]);
            _center = center == null ? null : new Lazy<MiraVent>(() => data.Vents[center.Value]);
            _right = right == null ? null : new Lazy<MiraVent>(() => data.Vents[right.Value]);
        }

        public enum Ids
        {
            Balcony = 1,
            Cafeteria = 2,
            Reactor = 3,
            Laboratory = 4,
            Office = 5,
            Admin = 6,
            Greenhouse = 7,
            Medbay = 8,
            Decontamination = 9,
            LockerRoom = 10,
            Launchpad = 11,
        }

        public Ids Id { get; }

        int IVent.Id => (int)Id;

        public string Name { get; }

        public Vector2 Position { get; }

        public MiraVent? Left => _left?.Value;

        IVent? IVent.Left => Left;

        public MiraVent? Center => _center?.Value;

        IVent? IVent.Center => Center;

        public MiraVent? Right => _right?.Value;

        IVent? IVent.Right => Right;
    }
}
