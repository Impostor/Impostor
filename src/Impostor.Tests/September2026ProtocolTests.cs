using System;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Innersloth.GameOptions.RoleOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel.Extensions;
using Impostor.Server.Net;
using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace Impostor.Tests;

public sealed class September2026ProtocolTests
{
    [Fact]
    public void CapturedSteamBuild7489SettingsRoundTrip()
    {
        // Captured with the 2026.9.29 Steam client's ToNetworkMessageWithSize.
        var wire = Convert.FromHexString("98010C9400000100640A00020000010000803F0000803F0000C03F00003E4200450001000000000178000000F0000000001401010002000B05000000030000000A08020000000200000F05040000000300003C0A00030000000200001E0F090000000200000F1E0A0000000300000F1E01080000000200000A010C00000001000003120000000100000F13000000010000321500000001000014");
        using var services = CreateServices();
        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        reader.Update(wire);

        var options = Assert.IsType<NormalGameOptions>(GameOptionsFactory.Deserialize(reader));

        Assert.Equal(12, options.Version);
        Assert.Equal(11, options.RoleOptions.Roles.Count);
        Assert.Equal(20f, Assert.IsType<SpiritGuideRoleOptions>(options.RoleOptions.Roles[RoleTypes.SpiritGuide].RoleOptions).SpiritGuideCooldownSeconds);
        Assert.Equal(0, reader.BytesRemaining);
        using var writer = MessageWriter.Get(MessageType.Reliable);
        GameOptionsFactory.Serialize(writer, options);
        // Impostor deliberately emits zero for the unused outer size; compare the entire payload.
        Assert.Equal(wire.AsSpan(2).ToArray(), writer.ToByteArray(includeHeader: false).AsSpan(1).ToArray());
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(1, true)]
    public async Task PlayerInfoConsumesUnknownTasksAndClearsPreviousLivingRole(int knownTasks, bool hasLivingRole)
    {
        using var services = CreateServices();
        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        using var writer = MessageWriter.Get(MessageType.Reliable);
        writer.Write((byte)4); // Player id.
        writer.WritePacked(42); // Client id.
        writer.Write((byte)0); // Outfits.
        writer.WritePacked(7u); // Level.
        writer.Write((byte)4); // Dead.
        writer.Write((ushort)RoleTypes.SpiritGuide);
        writer.Write(hasLivingRole);
        if (hasLivingRole)
        {
            writer.Write((ushort)RoleTypes.Scientist);
        }

        writer.Write((byte)2); // More tasks than the server has assigned.
        writer.WritePacked(128u);
        writer.Write(true);
        writer.WritePacked(256u);
        writer.Write(false);
        writer.Write("friend#1234");
        writer.Write("test-product-user");
        reader.Update(writer.ToByteArray(includeHeader: false));
        var info = new InnerPlayerInfo(null!, null!, null!, NullLogger<InnerPlayerInfo>.Instance)
        {
            RoleWhenAlive = RoleTypes.Engineer,
        };
        for (var i = 0; i < knownTasks; i++)
        {
            info.Tasks.Add(new TaskInfo(info, null!, 0, null));
        }

        var sender = new ClientPlayer(NullLogger<ClientPlayer>.Instance, new TestClient(), null!, 0);
        await info.DeserializeAsync(sender, null, reader, true);

        Assert.Equal(0, reader.BytesRemaining);
        Assert.Equal("friend#1234", info.FriendCode);
        Assert.Equal("test-product-user", info.ProductUserId);
        Assert.Equal(hasLivingRole ? RoleTypes.Scientist : (RoleTypes?)null, info.RoleWhenAlive);
        Assert.Null(info.RoleType); // Roles remain authoritative through SetRole.
        Assert.Equal(knownTasks, info.Tasks.Count);
        if (knownTasks > 0)
        {
            Assert.Equal(128u, info.Tasks[0].Id);
            Assert.True(info.Tasks[0].Complete);
        }
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    public void SpiritGuideOptionsUseOneByteCooldownAndReplacePreviousRoles(byte version)
    {
        using var services = CreateServices();
        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        // One role: ushort 21, count 1, chance 100, nested message with one byte cooldown.
        var wire = new byte[] { 1, 21, 0, 1, 100, 1, 0, 0, 20 };
        reader.Update(wire);
        var roles = new RoleOptionsCollection(version);
        roles.Roles[RoleTypes.Judge] = new(RoleTypes.Judge, new JudgeRoleOptions(version), new RoleRate(1, 50));

        roles.Deserialize(reader);

        Assert.Equal(0, reader.BytesRemaining);
        var role = Assert.Single(roles.Roles).Value;
        Assert.Equal(RoleTypes.SpiritGuide, role.Type);
        Assert.Equal(new RoleRate(1, 100), role.Rate);
        Assert.Equal(20f, Assert.IsType<SpiritGuideRoleOptions>(role.RoleOptions).SpiritGuideCooldownSeconds);
        using var writer = MessageWriter.Get(MessageType.Reliable);
        roles.Serialize(writer);
        Assert.Equal(wire, writer.ToByteArray(includeHeader: false));
    }

    [Theory]
    [InlineData(11, false)]
    [InlineData(12, false)]
    [InlineData(11, true)]
    [InlineData(12, true)]
    public void GameOptionsAcceptBothPreviousAndCurrentVersion(byte version, bool hideAndSeek)
    {
        using var services = CreateServices();
        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        using var writer = MessageWriter.Get(MessageType.Reliable);
        IGameOptions options = hideAndSeek ? new HideNSeekGameOptions(version) : new NormalGameOptions(version);
        GameOptionsFactory.Serialize(writer, options);
        writer.Write(0x12345678);
        reader.Update(writer.ToByteArray(includeHeader: false));

        var parsed = GameOptionsFactory.Deserialize(reader);

        Assert.Equal(version, parsed.Version);
        Assert.Equal(options.GameMode, parsed.GameMode);
        Assert.Equal(0x12345678, reader.ReadInt32());
        Assert.Equal(0, reader.BytesRemaining);
    }

    [Fact]
    public void SpiritGuideRpcUsesPackedLengthPrefixedImages()
    {
        using var services = CreateServices();
        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        var wire = new byte[] { 3, 2, 5, 9 };
        reader.Update(wire);

        Rpc67SpiritGuideMessage.Deserialize(reader, out var images);

        Assert.Equal(new byte[] { 2, 5, 9 }, images.ToArray());
        Assert.Equal(0, reader.BytesRemaining);
        using var writer = MessageWriter.Get(MessageType.Reliable);
        Rpc67SpiritGuideMessage.Serialize(writer, images.ToArray());
        Assert.Equal(wire, writer.ToByteArray(includeHeader: false));
    }

    private static ServiceProvider CreateServices()
    {
        var services = new ServiceCollection();
        services.AddHazel();
        return services.BuildServiceProvider();
    }

    private sealed class TestClient : ClientBase
    {
        public TestClient()
            : base("test", new GameVersion(2026, 7, 20), Language.English, QuickChatModes.FreeChatOrQuickChat, new PlatformSpecificData(Platforms.StandaloneSteamPC, "Steam"), null!)
        {
        }

        public override ValueTask HandleMessageAsync(IMessageReader message, MessageType messageType) => ValueTask.CompletedTask;

        public override ValueTask HandleDisconnectAsync(string reason) => ValueTask.CompletedTask;
    }
}
