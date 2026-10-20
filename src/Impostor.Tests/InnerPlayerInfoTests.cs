using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel.Extensions;
using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Xunit;

namespace Impostor.Tests;

public sealed class InnerPlayerInfoTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 1)]
    [InlineData(2, 2)]
    [InlineData(1, 3)]
    [InlineData(255, 1)]
    public async Task DeserializeConsumesAllTaskRecords(int wireTaskCount, int knownTaskCount)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHazel();
        using var services = serviceCollection.BuildServiceProvider();
        var game = new Game(
            NullLogger<Game>.Instance,
            null!,
            null!,
            new IPEndPoint(IPAddress.Loopback, 22023),
            new GameCode(123),
            new NormalGameOptions(),
            new GameFilterOptions(),
            null!,
            null!,
            null!,
            Options.Create(new CompatibilityConfig()),
            Options.Create(new TimeoutConfig()));
        var playerInfo = new InnerPlayerInfo(null!, null!, game, NullLogger<InnerPlayerInfo>.Instance);
        for (var i = 0; i < knownTaskCount; i++)
        {
            playerInfo.Tasks.Add(new TaskInfo(playerInfo, null!, (uint)i, null));
        }

        var originalTasks = playerInfo.Tasks.ToArray();
        using var writer = MessageWriter.Get(MessageType.Unreliable);
        writer.Write((byte)7); // PlayerId
        writer.WritePacked(42); // ClientId
        writer.Write((byte)0); // Outfits
        writer.WritePacked(10u); // PlayerLevel
        writer.Write((byte)0); // Flags
        writer.Write((ushort)RoleTypes.Crewmate);
        writer.Write(false); // RoleWhenAlive.HasValue
        writer.Write((byte)wireTaskCount);
        for (var i = 0; i < wireTaskCount; i++)
        {
            writer.WritePacked((uint)(128 + i));
            writer.Write(i % 2 == 0);
        }

        writer.Write("friend#1234");
        writer.Write("player-puid");
        var playerInfoLength = writer.ToByteArray(false).Length;
        writer.Write("following-data");

        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        reader.Update(writer.ToByteArray(false));
        await playerInfo.DeserializeAsync(null!, null, reader, false);

        Assert.Equal(playerInfoLength, reader.Position);
        Assert.Equal("following-data", reader.ReadString());
        Assert.Equal(reader.Length, reader.Position);
        Assert.Equal(knownTaskCount, playerInfo.Tasks.Count);
        for (var i = 0; i < knownTaskCount; i++)
        {
            Assert.Same(originalTasks[i], playerInfo.Tasks[i]);
            Assert.Equal(i < wireTaskCount ? (uint)(128 + i) : (uint)i, playerInfo.Tasks[i].Id);
            Assert.Equal(i < wireTaskCount && i % 2 == 0, playerInfo.Tasks[i].Complete);
        }
    }
}
