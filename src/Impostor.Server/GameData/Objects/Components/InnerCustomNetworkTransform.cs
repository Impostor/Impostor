using System;
using System.Numerics;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;

namespace Impostor.Server.GameData.Objects.Components
{
    public class InnerCustomNetworkTransform : InnerNetObject
    {
        private static readonly FloatRange XRange = new FloatRange(-40f, 40f);
        private static readonly FloatRange YRange = new FloatRange(-40f, 40f);

        private ushort _lastSequenceId;
        private Vector2 _targetSyncPosition;
        private Vector2 _targetSyncVelocity;

        public override void HandleRpc(byte callId, IMessageReader reader)
        {
            throw new NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                _lastSequenceId = reader.ReadUInt16();
                _targetSyncPosition = ReadVector2(reader);
                _targetSyncVelocity = ReadVector2(reader);

                Console.WriteLine(_targetSyncPosition + " - " + _targetSyncVelocity);
            }
            else
            {
                var newSid = reader.ReadUInt16();
                if (!SidGreaterThan(newSid, _lastSequenceId))
                {
                    return;
                }

                _lastSequenceId = newSid;
                _targetSyncPosition = ReadVector2(reader);
                _targetSyncVelocity = ReadVector2(reader);

                // TODO: Snap / update position

                Console.WriteLine(_targetSyncPosition + " - " + _targetSyncVelocity);
            }
        }

        private static bool SidGreaterThan(ushort newSid, ushort prevSid)
        {
            var num = (ushort)(prevSid + (uint) short.MaxValue);

            return (int) prevSid < (int) num
                ? newSid > prevSid && newSid <= num
                : newSid > prevSid || newSid <= num;
        }

        public static Vector2 ReadVector2(IMessageReader reader)
        {
            var v1 = (float) reader.ReadUInt16() / (float) ushort.MaxValue;
            var v2 = (float) reader.ReadUInt16() / (float) ushort.MaxValue;

            return new Vector2(XRange.Lerp(v1), YRange.Lerp(v2));
        }
    }
}