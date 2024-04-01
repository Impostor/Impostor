using System;

namespace Impostor.Api.Innersloth.GameOptions;

public static class GameOptionsFactory
{
    public const byte ModularOptionsDataVersion = 7;

    public static void Serialize(IMessageWriter writer, IGameOptions gameOptions)
    {
        if (gameOptions.Version < ModularOptionsDataVersion)
        {
            throw new NotSupportedException();
        }

        // In theory we should use WriteBytesAndSize here, but it's costly and requires computing the size
        // Base game does similar hacks and completely ignores this value for modular game options so we can just write 0 here
        writer.WritePacked(0);

        writer.Write(gameOptions.Version);
        writer.StartMessage(0);
        writer.Write((byte)gameOptions.GameMode);
        gameOptions.Serialize(writer);
        writer.EndMessage();
    }

    public static IGameOptions Deserialize(IMessageReader reader)
    {
        reader.ReadPackedInt32();
        var version = reader.ReadByte();

        if (version < ModularOptionsDataVersion)
        {
            return LegacyGameOptionsData.Deserialize(reader, version);
        }

        var optionsReader = reader.ReadMessage();
        var gameMode = (GameModes)optionsReader.ReadByte();

        return gameMode switch
        {
            GameModes.Normal or GameModes.NormalFools => NormalGameOptions.Deserialize(optionsReader, version),
            GameModes.HideNSeek or GameModes.SeekFools => HideNSeekGameOptions.Deserialize(optionsReader, version),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static void DeserializeInto(IMessageReader reader, IGameOptions gameOptions)
    {
        reader.ReadPackedInt32();
        var version = reader.ReadByte();

        if (version < ModularOptionsDataVersion)
        {
            ((LegacyGameOptionsData)gameOptions).Deserialize(reader);
            return;
        }

        var optionsReader = reader.ReadMessage();
        var gameMode = (GameModes)optionsReader.ReadByte();

        switch (gameMode)
        {
            case GameModes.Normal:
            case GameModes.NormalFools:
                ((NormalGameOptions)gameOptions).Deserialize(optionsReader);
                break;
            case GameModes.HideNSeek:
            case GameModes.SeekFools:
                ((HideNSeekGameOptions)gameOptions).Deserialize(optionsReader);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }
}
