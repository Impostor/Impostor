using System.Numerics;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.GameData.Objects.Components
{
    public class InnerCustomNetworkTransform : InnerNetObject
    {
        private static readonly FloatRange XRange = new FloatRange(-40f, 40f);
        private static readonly FloatRange YRange = new FloatRange(-40f, 40f);

        private readonly ILogger<InnerCustomNetworkTransform> _logger;

        private ushort _lastSequenceId;
        private Vector2 _targetSyncPosition;
        private Vector2 _targetSyncVelocity;

        public InnerCustomNetworkTransform(ILogger<InnerCustomNetworkTransform> logger)
        {
            _logger = logger;
        }

        private static bool SidGreaterThan(ushort newSid, ushort prevSid)
        {
            var num = (ushort)(prevSid + (uint) short.MaxValue);

            return (int) prevSid < (int) num
                ? newSid > prevSid && newSid <= num
                : newSid > prevSid || newSid <= num;
        }

        private static void WriteVector2(IMessageWriter writer, Vector2 vec)
        {
            writer.Write((ushort)(XRange.ReverseLerp(vec.X) * (double) ushort.MaxValue));
            writer.Write((ushort)(YRange.ReverseLerp(vec.Y) * (double) ushort.MaxValue));
        }

        private static Vector2 ReadVector2(IMessageReader reader)
        {
            var v1 = reader.ReadUInt16() / (float) ushort.MaxValue;
            var v2 = reader.ReadUInt16() / (float) ushort.MaxValue;

            return new Vector2(XRange.Lerp(v1), YRange.Lerp(v2));
        }

        public override void HandleRpc(byte callId, IMessageReader reader)
        {
            if (callId == 0)
            {
                SnapTo(ReadVector2(reader), reader.ReadUInt16());
            }
            else
            {
                _logger.LogWarning("InnerCustomNetworkTransform: Unknown rpc call {0}", callId);
            }
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.Write(_lastSequenceId);
                WriteVector2(writer, _targetSyncPosition);
                WriteVector2(writer, _targetSyncVelocity);
                return true;
            }

            // TODO: DirtyBits == 0 return false.
            _lastSequenceId++;

            writer.Write(_lastSequenceId);
            WriteVector2(writer, _targetSyncPosition);
            WriteVector2(writer, _targetSyncVelocity);
            return true;
        }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            var sequenceId = reader.ReadUInt16();

            if (initialState)
            {
                _lastSequenceId = sequenceId;
                _targetSyncPosition = ReadVector2(reader);
                _targetSyncVelocity = ReadVector2(reader);
            }
            else
            {
                if (!SidGreaterThan(sequenceId, _lastSequenceId))
                {
                    return;
                }

                _lastSequenceId = sequenceId;
                _targetSyncPosition = ReadVector2(reader);
                _targetSyncVelocity = ReadVector2(reader);
            }
        }

        private void SnapTo(Vector2 position, ushort minSid)
        {
            if (SidGreaterThan(minSid, _lastSequenceId))
            {
                return;
            }

            _lastSequenceId = minSid;
            _targetSyncPosition = position;
            _targetSyncVelocity = Vector2.Zero;
        }
    }
}