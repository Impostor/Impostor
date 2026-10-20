using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace Impostor.Tests;

public sealed class GameOptionsFactoryTests
{
    public static IEnumerable<object[]> ModularGameModes()
    {
        for (var version = GameOptionsFactory.ModularOptionsDataVersion; version <= NormalGameOptions.LatestVersion; version++)
        {
            yield return new object[] { version, GameModes.Normal };
            yield return new object[] { version, GameModes.NormalFools };
            yield return new object[] { version, GameModes.HideNSeek };
            yield return new object[] { version, GameModes.SeekFools };
        }
    }

    [Theory]
    [MemberData(nameof(ModularGameModes))]
    public void DeserializePreservesGameMode(byte version, GameModes gameMode)
    {
        var services = new ServiceCollection();
        services.AddHazel();
        using var provider = services.BuildServiceProvider();
        using var reader = provider.GetRequiredService<ObjectPool<MessageReader>>().Get();
        var payload = CreatePayload(version, gameMode);
        reader.Update(payload);

        var options = GameOptionsFactory.Deserialize(reader);

        Assert.Equal(gameMode, options.GameMode);
        Assert.Equal(version, options.Version);
        using var writer = MessageWriter.Get(MessageType.Unreliable);
        GameOptionsFactory.Serialize(writer, options);
        Assert.Equal(payload, writer.ToByteArray(false));
    }

    [Theory]
    [MemberData(nameof(ModularGameModes))]
    public void DeserializeIntoPreservesGameModeChanges(byte version, GameModes gameMode)
    {
        var services = new ServiceCollection();
        services.AddHazel();
        using var provider = services.BuildServiceProvider();
        using var reader = provider.GetRequiredService<ObjectPool<MessageReader>>().Get();
        var previousMode = gameMode switch
        {
            GameModes.Normal => GameModes.NormalFools,
            GameModes.NormalFools => GameModes.Normal,
            GameModes.HideNSeek => GameModes.SeekFools,
            _ => GameModes.HideNSeek,
        };
        reader.Update(CreatePayload(version, previousMode));
        var options = GameOptionsFactory.Deserialize(reader);
        var payload = CreatePayload(version, gameMode);
        reader.Update(payload);

        GameOptionsFactory.DeserializeInto(reader, options);

        Assert.Equal(gameMode, options.GameMode);
        Assert.Equal(version, options.Version);
        using var writer = MessageWriter.Get(MessageType.Unreliable);
        GameOptionsFactory.Serialize(writer, options);
        Assert.Equal(payload, writer.ToByteArray(false));
    }

    [Fact]
    public void NewOptionsUseStandardGameModes()
    {
        Assert.Equal(GameModes.Normal, new NormalGameOptions().GameMode);
        Assert.Equal(GameModes.HideNSeek, new HideNSeekGameOptions().GameMode);
    }

    private static byte[] CreatePayload(byte version, GameModes gameMode)
    {
        IGameOptions options = gameMode is GameModes.Normal or GameModes.NormalFools
            ? new NormalGameOptions(version)
            : new HideNSeekGameOptions(version);
        using var writer = MessageWriter.Get(MessageType.Unreliable);
        writer.WritePacked(0);
        writer.Write(version);
        writer.StartMessage(0);
        writer.Write((byte)gameMode);
        options.Serialize(writer);
        writer.EndMessage();
        return writer.ToByteArray(false);
    }
}
