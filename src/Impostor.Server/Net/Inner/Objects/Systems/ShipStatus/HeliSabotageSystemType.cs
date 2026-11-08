using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class HeliSabotageSystemType : ISystemType, IActivatable
    {
        private readonly HashSet<ActiveConsoleData> _activeConsoles = new();
        private readonly HashSet<byte> _completedConsoles = new();

        public HeliSabotageSystemType()
        {
            Countdown = 10000f;
            _completedConsoles.Add(0);
            _completedConsoles.Add(1);
        }

        public float Countdown { get; private set; }

        public float Timer { get; private set; }

        public bool IsActive => _completedConsoles.Count < 2;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(Countdown);
            writer.Write(Timer);

            writer.WritePacked(_activeConsoles.Count);
            foreach (var activeConsole in _activeConsoles)
            {
                writer.Write(activeConsole.PlayerId);
                writer.Write(activeConsole.ConsoleId);
            }

            writer.WritePacked(_completedConsoles.Count);
            foreach (var completedConsole in _completedConsoles)
            {
                writer.Write(completedConsole);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();
            Timer = reader.ReadSingle();
            _activeConsoles.Clear();
            _completedConsoles.Clear();

            var activeCount = reader.ReadPackedUInt32();

            for (var i = 0; i < activeCount; i++)
            {
                _activeConsoles.Add(new ActiveConsoleData(reader.ReadByte(), reader.ReadByte()));
            }

            var completedCount = reader.ReadPackedUInt32();

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
                    Countdown = 90f;
                    _completedConsoles.Clear();
                    _activeConsoles.Clear();
                    break;
                case 0x40 when playerControl != null:
                    _activeConsoles.Add(new ActiveConsoleData(playerControl.PlayerId, consoleId));
                    break;
                case 0x20 when playerControl != null:
                    _activeConsoles.Remove(new ActiveConsoleData(playerControl.PlayerId, consoleId));
                    break;
                case 0x10:
                    Timer = 10f;
                    _completedConsoles.Add(consoleId);
                    break;
            }
        }

        private readonly record struct ActiveConsoleData(byte PlayerId, byte ConsoleId);
    }
}
