using System.Numerics;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner;
using Impostor.Api.Unity;

namespace Impostor.Api.Net;

public static class MessageWriterExtensions
{
    public static void Write(this IMessageWriter writer, GameVersion value)
    {
        writer.Write(value.Value);
    }

    public static void Serialize(this GameCode gameCode, IMessageWriter writer)
    {
        writer.Write(gameCode.Value);
    }

    public static void Write(this IMessageWriter writer, IInnerNetObject? innerNetObject)
    {
        if (innerNetObject == null)
        {
            writer.Write(0);
        }
        else
        {
            writer.WritePacked(innerNetObject.NetId);
        }
    }

    public static void Write(this IMessageWriter writer, Vector2 vector)
    {
        writer.Write((ushort)(Mathf.ReverseLerp(vector.X) * (double)ushort.MaxValue));
        writer.Write((ushort)(Mathf.ReverseLerp(vector.Y) * (double)ushort.MaxValue));
    }
}
