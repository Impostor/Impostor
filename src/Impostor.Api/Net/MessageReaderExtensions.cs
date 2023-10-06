using System.Numerics;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner;
using Impostor.Api.Unity;

namespace Impostor.Api.Net;

public static class MessageReaderExtensions
{
    public static GameVersion ReadGameVersion(this IMessageReader reader)
    {
        return new GameVersion(reader.ReadInt32());
    }

    public static T? ReadNetObject<T>(this IMessageReader reader, IGame game)
        where T : IInnerNetObject
    {
        return game.FindObjectByNetId<T>(reader.ReadPackedUInt32());
    }

    public static Vector2 ReadVector2(this IMessageReader reader)
    {
        const float range = 50f;

        var x = reader.ReadUInt16() / (float)ushort.MaxValue;
        var y = reader.ReadUInt16() / (float)ushort.MaxValue;

        return new Vector2(Mathf.Lerp(-range, range, x), Mathf.Lerp(-range, range, y));
    }
}
