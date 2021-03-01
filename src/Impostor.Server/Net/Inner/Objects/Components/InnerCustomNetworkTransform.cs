using System.Collections.Generic;
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
        private Vector2 _targetSyncPosition;
        private Vector2 _targetSyncVelocity;

        private static Dictionary<RpcCalls, RpcInfo> Rpcs { get; } = new Dictionary<RpcCalls, RpcInfo>
        {
            [RpcCalls.SnapTo] = new RpcInfo(),
        };

        public InnerCustomNetworkTransform(ILogger<InnerCustomNetworkTransform> logger, InnerPlayerControl playerControl, Game game, IEventManager eventManager, ObjectPool<PlayerMovementEvent> pool)
        {
            _logger = logger;
            _playerControl = playerControl;
            _game = game;
            _game = game;
            _eventManager = eventManager;
            _pool = pool;
        }

        private static bool SidGreaterThan(ushort newSid, ushort prevSid)
        {
            var num = (ushort)(prevSid + (uint)short.MaxValue);

            return (int)prevSid < (int)num
                ? newSid > prevSid && newSid <= num
                : newSid > prevSid || newSid <= num;
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(_lastSequenceId);
                writer.Write(_targetSyncPosition);
                writer.Write(_targetSyncVelocity);
                return new ValueTask<bool>(true);
            }

            // TODO: DirtyBits == 0 return false.
            _lastSequenceId++;

            writer.Write(_lastSequenceId);
            writer.Write(_targetSyncPosition);
            writer.Write(_targetSyncVelocity);
            return new ValueTask<bool>(true);
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            var sequenceId = reader.ReadUInt16();

            if (initialState)
            {
                _lastSequenceId = sequenceId;
                await SetPositionAsync(sender, reader.ReadVector2());
                _targetSyncVelocity = reader.ReadVector2();
            }
            else
            {
                if (!sender.IsOwner(this))
                {
                    if (await sender.Client.ReportCheatAsync(CheatContext.Deserialize, $"Client attempted to send unowned {nameof(InnerCustomNetworkTransform)} data"))
                    {
                        return;
                    }
                }

                if (target != null)
                {
                    if (await sender.Client.ReportCheatAsync(CheatContext.Deserialize, $"Client attempted to send {nameof(InnerCustomNetworkTransform)} data to a specific player, must be broadcast"))
                    {
                        return;
                    }
                }

                if (!SidGreaterThan(sequenceId, _lastSequenceId))
                {
                    return;
                }

                _lastSequenceId = sequenceId;
                await SetPositionAsync(sender, reader.ReadVector2());
                _targetSyncVelocity = reader.ReadVector2();
            }
        }

        public override async ValueTask<bool> HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await TestRpc(sender, target, call, Rpcs))
            {
                return false;
            }

            if (call == RpcCalls.SnapTo)
            {
                if (!_playerControl.PlayerInfo.IsImpostor)
                {
                    if (await sender.Client.ReportCheatAsync(CheatContext.Deserialize, $"Client sent {nameof(RpcCalls.SnapTo)} as crewmate"))
                    {
                        return false;
                    }
                }

                Rpc21SnapTo.Deserialize(reader, out var position, out var minSid);

                SnapTo(position, minSid);
            }

            return true;
        }

        internal async ValueTask SetPositionAsync(IClientPlayer sender, Vector2 position)
        {
            _targetSyncPosition = position;

            var playerMovementEvent = _pool.Get();
            playerMovementEvent.Reset(_game, sender, _playerControl);
            await _eventManager.CallAsync(playerMovementEvent);
            _pool.Return(playerMovementEvent);
        }

        private void SnapTo(Vector2 position, ushort minSid)
        {
            if (!SidGreaterThan(minSid, _lastSequenceId))
            {
                return;
            }

            _lastSequenceId = minSid;
            _targetSyncPosition = position;
            _targetSyncVelocity = Vector2.Zero;
        }
    }
}
