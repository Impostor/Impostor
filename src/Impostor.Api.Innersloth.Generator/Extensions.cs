using System.Numerics;

namespace Impostor.Api.Innersloth.Generator;

internal static class Extensions
{
    public static string ToCSharpString(this Vector2 value)
    {
        return $"new Vector2({value.X}f, {value.Y}f)";
    }
}
