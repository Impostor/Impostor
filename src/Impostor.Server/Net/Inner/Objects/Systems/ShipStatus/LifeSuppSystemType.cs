using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class LifeSuppSystemType : ISystemType, IActivatable
    {
        private readonly float _lifeSuppDuration;

        public LifeSuppSystemType(float lifeSuppDuration = 30f)
        {
            _lifeSuppDuration = lifeSuppDuration;
            Countdown = 10000f;
            CompletedConsoles = new HashSet<int>();
        }

        public float Countdown { get; private set; }

        public HashSet<int> CompletedConsoles { get; }

        public bool IsActive => Countdown < 10000.0f;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(Countdown);
            writer.WritePacked(CompletedConsoles.Count);
            foreach (var consoleId in CompletedConsoles)
            {
                writer.WritePacked(consoleId);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();

            if (reader.Position >= reader.Length)
            {
                return;
            }

            CompletedConsoles.Clear();

            var num = reader.ReadPackedInt32();

            for (var i = 0; i < num; i++)
            {
                CompletedConsoles.Add(reader.ReadPackedInt32());
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            UpdateSystem(playerControl, reader.ReadByte());
        }

        internal void UpdateSystem(IInnerPlayerControl? playerControl, byte amount)
        {
            var value = amount;
            var consoleId = value & 3;

            if (value == 0x80 && !IsActive)
            {
                Countdown = _lifeSuppDuration;
                CompletedConsoles.Clear();
            }
            else if (value == 0x10)
            {
                Countdown = 10000f;
            }
            else if ((value & 0x40) != 0)
            {
                CompletedConsoles.Add(consoleId);
                if (CompletedConsoles.Count >= 2)
                {
                    Countdown = 10000f;
                }
            }
        }
    }
}
