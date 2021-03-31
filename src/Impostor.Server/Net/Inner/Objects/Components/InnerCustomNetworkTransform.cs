using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerCustomNetworkTransform : InnerNetObject
    {
        private readonly ILogger<InnerCustomNetworkTransform> _logger;
        private readonly InnerPlayerControl _playerControl;
        private readonly Game _game;
        private readonly IEventManager _eventManager;
        private readonly ObjectPool<PlayerMovementEvent> _pool;

        private ushort _lastSequenceId;

        public InnerCustomNetworkTransform(ILogger<InnerCustomNetworkTransform> logger, InnerPlayerControl playerControl, Game game, IEventManager eventManager, ObjectPool<PlayerMovementEvent> pool)
        {
            _logger = logger;
            _playerControl = playerControl;
            _game = game;
            _game = game;
            _eventManager = eventManager;
            _pool = pool;
        }

        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; private set; }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(_lastSequenceId);
                writer.Write(Position);
                writer.Write(Velocity);
                return new ValueTask<bool>(true);
            }

            // TODO: DirtyBits == 0 return false.
            _lastSequenceId++;

            writer.Write(_lastSequenceId);
            writer.Write(Position);
            writer.Write(Velocity);
            return new ValueTask<bool>(true);
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            var sequenceId = reader.ReadUInt16();

            if (initialState)
            {
                _lastSequenceId = sequenceId;
                await SetPositionAsync(sender, reader.ReadVector2(), reader.ReadVector2());
            }
            else
            {
                if (!await ValidateOwnership(CheatContext.Deserialize, sender) || !await ValidateBroadcast(CheatContext.Deserialize, sender, target))
                {
                    return;
                }

                if (!SidGreaterThan(sequenceId, _lastSequenceId))
                {
                    return;
                }

                _lastSequenceId = sequenceId;
                await SetPositionAsync(sender, reader.ReadVector2(), reader.ReadVector2());
            }
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (call == RpcCalls.SnapTo)
            {
                if (!await ValidateOwnership(call, sender) || !await ValidateImpostor(RpcCalls.MurderPlayer, sender, _playerControl.PlayerInfo))
                {
                    return false;
                }

                Rpc21SnapTo.Deserialize(reader, out var position, out var minSid);

                await SnapToAsync(sender, position, minSid);
                return true;
            }

            return await UnregisteredCall(call, sender);
        }

        internal async ValueTask SetPositionAsync(IClientPlayer sender, Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;

            var playerMovementEvent = _pool.Get();
            playerMovementEvent.Reset(_game, sender, _playerControl);
            await _eventManager.CallAsync(playerMovementEvent);
            _pool.Return(playerMovementEvent);
        }

        private static bool SidGreaterThan(ushort newSid, ushort prevSid)
        {
            var num = (ushort)(prevSid + (uint)short.MaxValue);

            return (int)prevSid < (int)num
                ? newSid > prevSid && newSid <= num
                : newSid > prevSid || newSid <= num;
        }

        private ValueTask SnapToAsync(IClientPlayer sender, Vector2 position, ushort minSid)
        {
            if (!SidGreaterThan(minSid, _lastSequenceId))
            {
                return default;
            }

            _lastSequenceId = minSid;
            return SetPositionAsync(sender, position, Velocity);
        }
    }
}
