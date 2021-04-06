using System.Numerics;

namespace Impostor.Api.Innersloth
{
    public abstract class Vent
    {
        internal Vent(int id, string name, Vector2 position)
        {
            Id = id;
            Name = name;
            Position = position;
        }

        public int Id { get; }

        public string Name { get; }

        public Vector2 Position { get; }

        public Vent? Left { get; internal set; }

        public Vent? Center { get; internal set; }

        public Vent? Right { get; internal set; }
    }
}
