using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel.Extensions;
using Impostor.Server;
using Impostor.Server.Events;
using Impostor.Server.Net;
using Impostor.Server.Net.Custom;
using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Xunit;

namespace Impostor.Tests;

public sealed class InnerPlayerControlTaskTests
{
    [Fact]
    public async Task CompleteTaskRpcMatchesTaskIdInsteadOfListPosition()
    {
        var customMessageManager = new CustomMessageManager<ICustomRpc>();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHazel();
        serviceCollection.AddLogging();
        serviceCollection.AddEventPools();
        serviceCollection.AddSingleton<IEventManager, EventManager>();
        serviceCollection.AddSingleton<ICustomMessageManager<ICustomRpc>>(customMessageManager);
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
        var eventManager = services.GetRequiredService<IEventManager>();
        var playerInfo = new InnerPlayerInfo(customMessageManager, eventManager, game, NullLogger<InnerPlayerInfo>.Instance);
        var firstTask = new TaskInfo(playerInfo, eventManager, 8, null);
        var requestedTask = new TaskInfo(playerInfo, eventManager, 0, null);
        playerInfo.Tasks.Add(firstTask);
        playerInfo.Tasks.Add(requestedTask);

        var playerControl = new InnerPlayerControl(
            customMessageManager,
            game,
            NullLogger<InnerPlayerControl>.Instance,
            services,
            eventManager,
            new RealDateTimeProvider())
        {
            OwnerId = 42,
            PlayerInfo = playerInfo,
        };
        var client = new TestClient { Id = 42 };
        var sender = new ClientPlayer(NullLogger<ClientPlayer>.Instance, client, game, 0);

        using var writer = MessageWriter.Get(MessageType.Unreliable);
        Rpc01CompleteTask.Serialize(writer, requestedTask.Id);
        using var reader = services.GetRequiredService<ObjectPool<MessageReader>>().Get();
        reader.Update(writer.ToByteArray(false));

        Assert.True(await playerControl.HandleRpcAsync(sender, null, RpcCalls.CompleteTask, reader));
        Assert.False(firstTask.Complete);
        Assert.True(requestedTask.Complete);
    }

    private sealed class TestClient : ClientBase
    {
        public TestClient()
            : base("test", default, default, default, null!, null!)
        {
        }

        public override ValueTask HandleMessageAsync(IMessageReader message, MessageType messageType) => default;

        public override ValueTask HandleDisconnectAsync(string reason) => default;
    }
}
