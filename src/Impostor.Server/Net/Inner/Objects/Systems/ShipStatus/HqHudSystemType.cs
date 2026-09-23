using System;
using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class HqHudSystemType : ISystemType, IActivatable
    {
        private readonly HashSet<Tuple<byte, byte>> _activeConsoles = new();
        private readonly HashSet<byte> _completedConsoles = new();

        public HqHudSystemType()
        {
            _completedConsoles.Add(0);
            _completedConsoles.Add(1);
        }

        public float Timer { get; private set; }

        public bool IsActive => _completedConsoles.Count < 2;

        public float NumComplete => _completedConsoles.Count;

        public float PercentActive => Timer / 10f;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.WritePacked(_activeConsoles.Count);
            foreach (var activeConsole in _activeConsoles)
            {
                writer.Write(activeConsole.Item1);
                writer.Write(activeConsole.Item2);
            }

            writer.WritePacked(_completedConsoles.Count);
            foreach (var completedConsole in _completedConsoles)
            {
                writer.Write(completedConsole);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            _activeConsoles.Clear();
            var activeCount = reader.ReadPackedInt32();
            for (var i = 0; i < activeCount; i++)
            {
                _activeConsoles.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
            }

            _completedConsoles.Clear();
            var completedCount = reader.ReadPackedInt32();
            for (var i = 0; i < completedCount; i++)
            {
                _completedConsoles.Add(reader.ReadByte());
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            UpdateSystem(playerControl, reader.ReadByte());
        }

        internal void UpdateSystem(IInnerPlayerControl? playerControl, byte amount)
        {
            var value = amount;
            var consoleId = (byte)(value & 0x0f);

            switch (value & 0xf0)
            {
                case 0x80:
                    Timer = -1f;
                    _completedConsoles.Clear();
                    _activeConsoles.Clear();
                    break;
                case 0x40 when playerControl != null:
                    _activeConsoles.Add(new Tuple<byte, byte>(playerControl.PlayerId, consoleId));
                    break;
                case 0x20 when playerControl != null:
                    _activeConsoles.Remove(new Tuple<byte, byte>(playerControl.PlayerId, consoleId));
                    break;
                case 0x10:
                    Timer = 10f;
                    _completedConsoles.Add(consoleId);
                    break;
            }
        }

        internal bool IsConsoleActive(int consoleId)
        {
            return _activeConsoles.Any(x => x.Item2 == (byte)consoleId);
        }

        internal bool IsConsoleOkay(int consoleId)
        {
            return _completedConsoles.Contains((byte)consoleId);
        }
    }
}
