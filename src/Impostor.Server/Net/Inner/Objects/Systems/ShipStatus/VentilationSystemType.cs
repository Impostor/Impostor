using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class VentilationSystemType : ISystemType
    {
        private readonly Dictionary<byte, SequenceBuffer<VentMoveInfo>> _sequenceBuffers = new();
        private readonly Dictionary<byte, byte> _playersCleaningVents = new();
        private readonly Dictionary<byte, byte> _playersInsideVents = new();

        public enum Operation : byte
        {
            StartCleaning,
            StopCleaning,
            Enter,
            Exit,
            Move,
            BootImpostors,
        }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.WritePacked(_playersCleaningVents.Count);
            foreach (var pair in _playersCleaningVents)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }

            writer.WritePacked(_playersInsideVents.Count);
            foreach (var pair in _playersInsideVents)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            _playersCleaningVents.Clear();
            var cleaningCount = reader.ReadPackedInt32();
            for (var i = 0; i < cleaningCount; i++)
            {
                _playersCleaningVents[reader.ReadByte()] = reader.ReadByte();
            }

            _playersInsideVents.Clear();
            var insideCount = reader.ReadPackedInt32();
            for (var i = 0; i < insideCount; i++)
            {
                _playersInsideVents[reader.ReadByte()] = reader.ReadByte();
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            if (playerControl == null)
            {
                return;
            }

            var opId = reader.ReadUInt16();
            var operation = (Operation)reader.ReadByte();
            var ventId = reader.ReadByte();
            var playerId = playerControl.PlayerId;

            if (!_sequenceBuffers.TryGetValue(playerId, out var sequenceBuffer))
            {
                sequenceBuffer = new SequenceBuffer<VentMoveInfo>((ushort)(opId - 1));
                _sequenceBuffers[playerId] = sequenceBuffer;
            }

            if (sequenceBuffer.IsInvalidSid(opId))
            {
                return;
            }

            if (sequenceBuffer.IsNextSid(opId))
            {
                PerformVentOp(playerId, operation, ventId, sequenceBuffer);
            }
            else
            {
                sequenceBuffer.Add(opId, new VentMoveInfo(operation, ventId, playerId));
            }

            foreach (var info in sequenceBuffer.SubsequentObjs())
            {
                PerformVentOp(info.PlayerId, info.Op, info.VentId, sequenceBuffer);
            }
        }

        public bool IsVentCurrentlyBeingCleaned(int ventId)
        {
            return _playersCleaningVents.Any(x => x.Value == ventId);
        }

        public bool IsImpostorInsideVent(int ventId)
        {
            return _playersInsideVents.Any(x => x.Value == ventId);
        }

        private void PerformVentOp(byte playerId, Operation operation, byte ventId, SequenceBuffer<VentMoveInfo> sequenceBuffer)
        {
            sequenceBuffer.BumpSid();
            switch (operation)
            {
                case Operation.StartCleaning:
                    BootImpostorsFromVent(ventId);
                    _playersCleaningVents[playerId] = ventId;
                    break;
                case Operation.StopCleaning:
                    foreach (var pair in _playersCleaningVents.ToArray())
                    {
                        if (pair.Value == ventId)
                        {
                            _playersCleaningVents.Remove(pair.Key);
                        }
                    }

                    break;
                case Operation.Enter:
                    _playersInsideVents[playerId] = ventId;
                    break;
                case Operation.Exit:
                    _playersInsideVents.Remove(playerId);
                    break;
                case Operation.Move:
                    _playersInsideVents[playerId] = ventId;
                    break;
                case Operation.BootImpostors:
                    BootImpostorsFromVent(ventId);
                    break;
            }
        }

        private void BootImpostorsFromVent(byte ventId)
        {
            foreach (var pair in _playersInsideVents.ToArray())
            {
                if (pair.Value == ventId)
                {
                    _playersInsideVents.Remove(pair.Key);
                }
            }
        }

        private readonly record struct VentMoveInfo(Operation Op, byte VentId, byte PlayerId);
    }
}
