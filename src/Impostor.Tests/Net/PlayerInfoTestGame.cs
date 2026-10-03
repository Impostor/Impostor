using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Manager;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel.Extensions;
using Impostor.Server.Events;
using Impostor.Server.Net;
using Impostor.Server.Net.Custom;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Xunit;

namespace Impostor.Tests.Net;

internal sealed class PlayerInfoTestGame : IDisposable
{
    private readonly ServiceProvider _services;
    private readonly ClientManager _clientManager;
    private readonly GameVersion _version;

    public PlayerInfoTestGame(bool hostAuthority = false)
    {
        _version = new GameVersion(2025, 1, 1, hostAuthority ? 25 : 0);
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IEventManager, EventManager>()
            .AddSingleton<ICustomMessageManager<ICustomRpc>, CustomMessageManager<ICustomRpc>>();
        services.AddHazel();
        _services = services.BuildServiceProvider();

        var eventManager = _services.GetRequiredService<IEventManager>();
        var compatibility = new CompatibilityManager(
            NullLogger<CompatibilityManager>.Instance,
            new ICompatibilityManager.CompatibilityGroup[] { new[] { _version.Normalize() } });
        var config = Options.Create(new CompatibilityConfig { AllowHostAuthority = true });
        _clientManager = new ClientManager(NullLogger<ClientManager>.Instance, eventManager, new TestClientFactory(), compatibility, config);
        Game = new Game(
            NullLogger<Game>.Instance,
            _services,
            null,
            new IPEndPoint(IPAddress.Loopback, 0),
            new GameCode("ABCDEF"),
            new NormalGameOptions(),
            GameFilterOptions.CreateDefault(),
            _clientManager,
            eventManager,
            compatibility,
            config,
            Options.Create(new TimeoutConfig { SpawnTimeout = -1 }));
    }

    public Game Game { get; }

    public async Task<TestClient> AddClientAsync()
    {
        var connection = new TestConnection();
        await _clientManager.RegisterConnectionAsync(
            connection,
            "Test",
            _version,
            Language.English,
            QuickChatModes.FreeChatOrQuickChat,
            new PlatformSpecificData(Platforms.StandaloneSteamPC, "Test"));
        var client = Assert.IsType<TestClient>(connection.Client);
        Assert.True((await Game.AddClientAsync(client)).IsSuccess);
        client.Player.DisableSpawnTimeout();
        return client;
    }

    public InnerPlayerInfo CreatePlayerInfo(int ownerId, int clientId = 1)
    {
        var playerInfo = ActivatorUtilities.CreateInstance<InnerPlayerInfo>(_services, Game);
        playerInfo.OwnerId = ownerId;
        playerInfo.ClientId = clientId;
        return playerInfo;
    }

    public MessageReader CreateReader(IMessageWriter writer)
    {
        var reader = _services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        reader.Update(writer.ToByteArray(false));
        return reader;
    }

    public void Dispose()
    {
        _services.Dispose();
    }

    internal sealed class TestClient : ClientBase
    {
        public TestClient(IHazelConnection connection, string name, GameVersion version, Language language, QuickChatModes chatMode, PlatformSpecificData platform)
            : base(name, version, language, chatMode, platform, connection)
        {
        }

        public bool RejectCheats { get; set; } = true;

        public List<CheatCategory> Reports { get; } = new();

        public HashSet<CheatCategory> IgnoredCategories { get; } = new();

        public override ValueTask<bool> ReportCheatAsync(CheatContext context, CheatCategory category, string message)
        {
            Reports.Add(category);
            return new ValueTask<bool>(RejectCheats && !IgnoredCategories.Contains(category));
        }

        public override ValueTask HandleMessageAsync(IMessageReader message, MessageType messageType) => ValueTask.CompletedTask;

        public override ValueTask HandleDisconnectAsync(string reason) => ValueTask.CompletedTask;
    }

    private sealed class TestClientFactory : IClientFactory
    {
        public ClientBase Create(IHazelConnection connection, string name, GameVersion clientVersion, Language language, QuickChatModes chatMode, PlatformSpecificData platformSpecificData)
        {
            var client = new TestClient(connection, name, clientVersion, language, chatMode, platformSpecificData);
            connection.Client = client;
            return client;
        }
    }

    private sealed class TestConnection : IHazelConnection
    {
        public IPEndPoint EndPoint { get; } = new(IPAddress.Loopback, 0);

        public bool IsConnected => true;

        public IClient Client { get; set; }

        public float AveragePing => 0;

        public ValueTask SendAsync(IMessageWriter writer) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(string reason, IMessageWriter writer = null) => ValueTask.CompletedTask;
    }
}
