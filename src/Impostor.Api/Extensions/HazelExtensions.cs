using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages.S2C;
using Impostor.Api.Unity;

namespace Impostor.Api;

public static class HazelExtensions
{
    /// <summary>
    ///     Disconnect a connection using a custom message.
    /// </summary>
    /// <param name="connection">The connection to disconnect.</param>
    /// <param name="reason">The reason to disconnect with.</param>
    /// <param name="message">
    ///     The custom message to disconnect with if <paramref name="reason" /> is
    ///     <see cref="DisconnectReason.Custom" />.
    /// </param>
    /// <returns>Task that should be awaited to ensure disconnection.</returns>
    public static async ValueTask CustomDisconnectAsync(this IHazelConnection connection, DisconnectReason reason,
        string? message = null)
    {
        if (!connection.IsConnected)
        {
            return;
        }

        using var writer = connection.GetWriter();
        MessageDisconnect.Serialize(writer, true, reason, message);

        await connection.DisconnectAsync(reason.ToString(), writer);
    }
    
    
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
        const float Range = 50f;

        var x = reader.ReadUInt16() / (float)ushort.MaxValue;
        var y = reader.ReadUInt16() / (float)ushort.MaxValue;

        return new Vector2(Mathf.Lerp(-Range, Range, x), Mathf.Lerp(-Range, Range, y));
    }
    
    
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
