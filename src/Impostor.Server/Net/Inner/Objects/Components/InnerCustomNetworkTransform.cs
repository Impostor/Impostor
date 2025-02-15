using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server.Net.Inner.Objects.Components;

internal partial class InnerCustomNetworkTransform(
    Game game,
    ILogger<InnerCustomNetworkTransform> logger,
    InnerPlayerControl playerControl,
    IEventManager eventManager,
    ObjectPool<PlayerMovementEvent> pool)
    : InnerNetObject(game)
{
    private static readonly Vector2 ColliderOffset = new(0f, -0.4f);

    private readonly ILogger<InnerCustomNetworkTransform> _logger = logger;

    private ushort _lastSequenceId;

    public Vector2 Position { get; private set; }

    public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
    {
        if (initialState)
        {
            writer.Write(_lastSequenceId);
            writer.Write(Position);
            return new ValueTask<bool>(true);
        }

        writer.Write(_lastSequenceId);

        // Impostor doesn't keep a memory of positions, so just send the last one
        writer.WritePacked(1);
        writer.Write(Position);
        return new ValueTask<bool>(true);
    }

    public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader,
        bool initialState)
    {
        var sequenceId = reader.ReadUInt16();

        if (initialState)
        {
            _lastSequenceId = sequenceId;
            await SetPositionAsync(sender, reader.ReadVector2());
        }
        else
        {
            if (!await ValidateOwnership(CheatContext.Deserialize, sender) ||
                !await ValidateBroadcast(CheatContext.Deserialize, sender, target))
            {
                return;
            }

            var positions = reader.ReadPackedInt32();

            for (var i = 0; i < positions; i++)
            {
                var position = reader.ReadVector2();
                var newSid = (ushort)(sequenceId + i);
                if (SidGreaterThan(newSid, _lastSequenceId))
                {
                    _lastSequenceId = newSid;
                    await SetPositionAsync(sender, position);
                }
            }
        }
    }

    public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call,
        IMessageReader reader)
    {
        if (call == RpcCalls.SnapTo)
        {
            if (!await ValidateOwnership(call, sender))
            {
                return false;
            }

            Rpc21SnapTo.Deserialize(reader, out var position, out var minSid);

            if (Game.GameNet.ShipStatus is { } shipStatus)
            {
                var vents = shipStatus.Data.Vents.Values;

                var vent = vents.SingleOrDefault(x => Approximately(x.Position, position + ColliderOffset));

                if (vent != null)
                {
                    if (!await ValidateCanVent(call, sender, playerControl.PlayerInfo))
                    {
                        return false;
                    }

                    await eventManager.CallAsync(new PlayerVentEvent(Game, sender, playerControl, vent));
                }
            }

            await SnapToAsync(sender, position, minSid);
            return true;
        }

        return await base.HandleRpcAsync(sender, target, call, reader);
    }

    internal async ValueTask SetPositionAsync(IClientPlayer sender, Vector2 position)
    {
        Position = position;

        var playerMovementEvent = pool.Get();
        playerMovementEvent.Reset(Game, sender, playerControl);
        await eventManager.CallAsync(playerMovementEvent);
        pool.Return(playerMovementEvent);
    }

    private static bool SidGreaterThan(ushort newSid, ushort prevSid)
    {
        var num = (ushort)(prevSid + (uint)short.MaxValue);

        return prevSid < num
            ? newSid > prevSid && newSid <= num
            : newSid > prevSid || newSid <= num;
    }

    private static bool Approximately(Vector2 a, Vector2 b, float tolerance = 0.1f)
    {
        var abs = Vector2.Abs(a - b);
        return abs.X <= tolerance && abs.Y <= tolerance;
    }

    private ValueTask SnapToAsync(IClientPlayer sender, Vector2 position, ushort minSid)
    {
        if (!SidGreaterThan(minSid, _lastSequenceId))
        {
            return default;
        }

        _lastSequenceId = minSid;
        return SetPositionAsync(sender, position);
    }

    private enum AirshipSpawnState : byte
    {
        PreSpawn,
        SelectingSpawn,
        Spawned,
    }
}
