using System;
using System.Numerics;

namespace Impostor.Api.Innersloth.Maps.Vents
{
    public class AirshipVent : IVent
    {
        private readonly Lazy<AirshipVent>? _left;

        private readonly Lazy<AirshipVent>? _center;

        private readonly Lazy<AirshipVent>? _right;

        internal AirshipVent(AirshipData data, Ids id, Vector2 position, Ids? left = null, Ids? center = null, Ids? right = null)
        {
            Id = id;
            Name = id.ToString();
            Position = position;

            _left = left == null ? null : new Lazy<AirshipVent>(() => data.Vents[left.Value]);
            _center = center == null ? null : new Lazy<AirshipVent>(() => data.Vents[center.Value]);
            _right = right == null ? null : new Lazy<AirshipVent>(() => data.Vents[right.Value]);
        }

        public enum Ids
        {
            Vault = 0,
            Cockpit = 1,
            ViewingDeck = 2,
            EngineRoom = 3,
            Kitchen = 4,
            MainHallBottom = 5,
            GapRight = 6,
            GapLeft = 7,
            MainHallTop = 8,
            Showers = 9,
            Records = 10,
            CargoBay = 11,
        }

        public Ids Id { get; }

        int IVent.Id => (int)Id;

        public string Name { get; }

        public Vector2 Position { get; }

        public AirshipVent? Left => _left?.Value;

        IVent? IVent.Left => Left;

        public AirshipVent? Center => _center?.Value;

        IVent? IVent.Center => Center;

        public AirshipVent? Right => _right?.Value;

        IVent? IVent.Right => Right;
    }
}
