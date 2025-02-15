using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Server.Events;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner;

internal abstract partial class InnerNetObject(Game game) : GameObject, IInnerNetObject
{
    private const int HostInheritId = -2;

    public Game Game { get; } = game;

    public SpawnFlags SpawnFlags { get; internal set; }

    public uint NetId { get; internal set; }

    public int OwnerId { get; internal set; }

    IGame IInnerNetObject.Game
    {
        get => Game;
    }

    public bool IsOwnedBy(IClientPlayer player)
    {
        return OwnerId == player.Client.Id ||
               (OwnerId == HostInheritId && player.IsHost);
    }

    public abstract ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState);

    public abstract ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader,
        bool initialState);

    public virtual async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call,
        IMessageReader reader)
    {
        return await UnregisteredCallAsync(call, sender);
    }
    
    internal virtual ValueTask OnSpawnAsync()
    {
        return default;
    }
}
