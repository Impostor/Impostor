using System.Numerics;

namespace Impostor.Api.Innersloth
{
    public interface IVent
    {
        int Id { get; }

        string Name { get; }

        Vector2 Position { get; }

        IVent? Left { get; }

        IVent? Center { get; }

        IVent? Right { get; }
    }
}
