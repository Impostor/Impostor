using System;
using System.Reflection;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Server.Net.Inner;
using Impostor.Server.Net.Inner.Objects;
using Xunit;

namespace Impostor.Tests.Net;

public sealed class PlayerInfoTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SceneChangePreservesServerPlayerInfoLifecycle(bool hostAuthority)
    {
        using var fixture = new PlayerInfoTestGame(hostAuthority);
        var host = await fixture.AddClientAsync();
        var guest = await fixture.AddClientAsync();

        foreach (var client in new[] { host, guest })
        {
            // A repeated scene change syncs existing objects without creating duplicates.
            for (var i = 0; i < 2; i++)
            {
                using var writer = MessageWriter.Get(MessageType.Reliable);
                writer.StartMessage(GameDataTag.SceneChangeFlag);
                writer.WritePacked(client.Id);
                writer.Write("OnlineGame");
                writer.EndMessage();
                using var reader = fixture.CreateReader(writer);

                Assert.True(await fixture.Game.HandleGameDataAsync(reader, client.Player, false));
                Assert.Empty(client.Reports);
            }
        }

        if (hostAuthority)
        {
            Assert.Empty(fixture.Game.GameNet.GameData.Players);
        }
        else
        {
            Assert.Equal(2, fixture.Game.GameNet.GameData.PlayerCount);
            foreach (var client in new[] { host, guest })
            {
                var info = fixture.Game.GameNet.GameData.PlayersByClientId[client.Id];
                Assert.Equal(-4, info.OwnerId);
                Assert.Same(info, fixture.Game.FindObjectByNetId<InnerPlayerInfo>(info.NetId));
            }
        }
    }

    [Theory]
    [InlineData(false, true, -2, CheatCategory.Ownership)]
    [InlineData(false, true, -4, CheatCategory.Ownership)]
    [InlineData(true, true, -4, CheatCategory.Ownership)]
    [InlineData(true, true, -3, CheatCategory.Ownership)]
    [InlineData(true, true, 1, CheatCategory.Ownership)]
    [InlineData(true, true, 2, CheatCategory.Ownership)]
    [InlineData(false, false, -2, CheatCategory.MustBeHost)]
    [InlineData(true, false, -2, CheatCategory.MustBeHost)]
    public async Task ClientSpawnIsRejectedBeforeReadingObjectData(bool hostAuthority, bool isHost, int ownerId, CheatCategory expectedCategory)
    {
        using var fixture = new PlayerInfoTestGame(hostAuthority);
        var host = await fixture.AddClientAsync();
        var sender = isHost ? host : await fixture.AddClientAsync();

        // This mock exposes only a spawn header. Any attempt to read object data fails.
        using var reader = DispatchProxy.Create<IMessageReader, SpawnHeaderReader>();
        ((SpawnHeaderReader)reader).OwnerId = ownerId;

        Assert.False(await fixture.Game.HandleGameDataAsync(reader, sender.Player, false));
        Assert.Equal(expectedCategory, Assert.Single(sender.Reports));
        Assert.Empty(fixture.Game.GameNet.GameData.Players);
    }

    [Fact]
    public async Task SpawnOwnershipIsCheckedWhenHostCheckIsIgnored()
    {
        using var fixture = new PlayerInfoTestGame(hostAuthority: true);
        await fixture.AddClientAsync();
        var guest = await fixture.AddClientAsync();
        guest.IgnoredCategories.Add(CheatCategory.MustBeHost);
        using var reader = DispatchProxy.Create<IMessageReader, SpawnHeaderReader>();
        ((SpawnHeaderReader)reader).OwnerId = -2;

        Assert.False(await fixture.Game.HandleGameDataAsync(reader, guest.Player, false));
        Assert.Equal(new[] { CheatCategory.MustBeHost, CheatCategory.Ownership }, guest.Reports);
        Assert.Empty(fixture.Game.GameNet.GameData.Players);
    }

    [Fact]
    public async Task HostAuthorityCanSpawnPlayerInfo()
    {
        using var fixture = new PlayerInfoTestGame(hostAuthority: true);
        var host = await fixture.AddClientAsync();
        var expected = fixture.CreatePlayerInfo(-2, host.Id);
        expected.PlayerId = 7;
        expected.PlayerLevel = 12;

        using var writer = MessageWriter.Get(MessageType.Reliable);
        writer.StartMessage(GameDataTag.SpawnFlag);
        writer.WritePacked(11u);
        writer.WritePacked(expected.OwnerId);
        writer.Write((byte)SpawnFlags.None);
        writer.WritePacked(1);
        writer.WritePacked(42u);
        writer.StartMessage(1);
        await expected.SerializeAsync(writer, true);
        writer.EndMessage();
        writer.EndMessage();
        using var reader = fixture.CreateReader(writer);

        Assert.True(await fixture.Game.HandleGameDataAsync(reader, host.Player, false));

        var actual = fixture.Game.FindObjectByNetId<InnerPlayerInfo>(42);
        Assert.NotNull(actual);
        Assert.Equal(-2, actual.OwnerId);
        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.PlayerLevel, actual.PlayerLevel);
        Assert.Same(actual, fixture.Game.GameNet.GameData.PlayersByClientId[host.Id]);
        Assert.Empty(host.Reports);
    }

    [Theory]
    [InlineData(-4, true, false)]
    [InlineData(-4, false, false)]
    [InlineData(-2, false, true)]
    [InlineData(2, true, true)]
    public async Task UnownedPlayerInfoIsRejectedWithoutReadingState(int ownerId, bool isHost, bool initialState)
    {
        using var fixture = new PlayerInfoTestGame(hostAuthority: true);
        var host = await fixture.AddClientAsync();
        var sender = isHost ? host : await fixture.AddClientAsync();
        var info = fixture.CreatePlayerInfo(ownerId);
        info.PlayerLevel = 10;

        // No reader is needed: rejected updates must return before consuming any state.
        await info.DeserializeAsync(sender.Player, null, null, initialState);

        Assert.Equal(CheatCategory.Ownership, Assert.Single(sender.Reports));
        Assert.Equal(10u, info.PlayerLevel);
    }

    [Theory]
    [InlineData(-2, true, true)]
    [InlineData(-2, true, false)]
    [InlineData(2, false, false)]
    public async Task OwnerCanDeserializePlayerInfo(int ownerId, bool isHost, bool initialState)
    {
        using var fixture = new PlayerInfoTestGame(hostAuthority: true);
        var host = await fixture.AddClientAsync();
        var sender = isHost ? host : await fixture.AddClientAsync();
        var expected = fixture.CreatePlayerInfo(ownerId, sender.Id);
        expected.PlayerId = 5;
        expected.PlayerLevel = 24;
        var actual = fixture.CreatePlayerInfo(ownerId, sender.Id);
        using var writer = MessageWriter.Get(MessageType.Reliable);
        await expected.SerializeAsync(writer, initialState);
        using var reader = fixture.CreateReader(writer);

        await actual.DeserializeAsync(sender.Player, null, reader, initialState);

        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.PlayerLevel, actual.PlayerLevel);
        Assert.Equal(reader.Length, reader.Position);
        Assert.Empty(sender.Reports);
    }

    [Fact]
    public async Task DeserializeContinuesWhenCheatReportIsIgnored()
    {
        using var fixture = new PlayerInfoTestGame();
        var host = await fixture.AddClientAsync();
        host.RejectCheats = false;
        var expected = fixture.CreatePlayerInfo(-4, host.Id);
        expected.PlayerLevel = 24;
        var actual = fixture.CreatePlayerInfo(-4, host.Id);
        using var writer = MessageWriter.Get(MessageType.Reliable);
        await expected.SerializeAsync(writer, false);
        using var reader = fixture.CreateReader(writer);

        await actual.DeserializeAsync(host.Player, null, reader, false);

        Assert.Equal(expected.PlayerLevel, actual.PlayerLevel);
        Assert.Equal(CheatCategory.Ownership, Assert.Single(host.Reports));
    }

    public class SpawnHeaderReader : DispatchProxy
    {
        public int OwnerId { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return targetMethod.Name switch
            {
                "get_Position" => 0,
                "get_Length" => 1,
                "get_Tag" => GameDataTag.SpawnFlag,
                nameof(IMessageReader.ReadMessage) => this,
                nameof(IMessageReader.ReadPackedUInt32) => 11u,
                nameof(IMessageReader.ReadPackedInt32) => OwnerId,
                nameof(IDisposable.Dispose) => null,
                _ => throw new InvalidOperationException($"Unexpected read after spawn header: {targetMethod.Name}"),
            };
        }
    }
}
