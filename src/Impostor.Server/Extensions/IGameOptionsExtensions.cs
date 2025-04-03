using System;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Hazel;

namespace Impostor.Server.Extensions;

public static class IGameOptionsExtensions
{
    public static byte[] ToBytes(this IGameOptions gameOptions)
    {
        using var writer = MessageWriter.Get(MessageType.Unreliable);
        writer.Write(gameOptions.Version);
        writer.StartMessage(0);
        writer.Write((byte)gameOptions.GameMode);
        gameOptions.Serialize(writer);
        writer.EndMessage();
        return writer.ToByteArray(false);
    }

    public static string ToBase64String(this IGameOptions gameOptions)
    {
        return Convert.ToBase64String(gameOptions.ToBytes());
    }
}
