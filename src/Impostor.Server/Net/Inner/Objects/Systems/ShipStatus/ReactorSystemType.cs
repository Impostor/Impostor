using System;
using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class ReactorSystemType : ISystemType, IActivatable
    {
        private readonly float _reactorDuration;

        public ReactorSystemType(float reactorDuration = 30f, SystemTypes system = SystemTypes.Reactor)
        {
            _reactorDuration = reactorDuration;
            System = system;
            Countdown = 10000f;
            UserConsolePairs = new HashSet<Tuple<byte, byte>>();
        }

        public SystemTypes System { get; }

        public float Countdown { get; private set; }

        public HashSet<Tuple<byte, byte>> UserConsolePairs { get; }

        public bool IsActive => Countdown < 10000.0f;

        public int UserCount
        {
            get
            {
                var result = 0;
                var consoles = 0;
                foreach (var pair in UserConsolePairs)
                {
                    var bit = 1 << pair.Item2;
                    if ((bit & consoles) == 0)
                    {
                        result++;
                        consoles |= bit;
                    }
                }

                return result;
            }
        }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(Countdown);
            writer.WritePacked(UserConsolePairs.Count);
            foreach (var pair in UserConsolePairs)
            {
                writer.Write(pair.Item1);
                writer.Write(pair.Item2);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();
            UserConsolePairs.Clear();

            var count = reader.ReadPackedInt32();

            for (var i = 0; i < count; i++)
            {
                UserConsolePairs.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            UpdateSystem(playerControl, reader.ReadByte());
        }

        internal void UpdateSystem(IInnerPlayerControl? playerControl, byte amount)
        {
            var value = amount;
            var consoleId = (byte)(value & 3);

            if (value == 0x80 && !IsActive)
            {
                Countdown = _reactorDuration;
                UserConsolePairs.Clear();
            }
            else if (value == 0x10)
            {
                Countdown = 10000f;
            }
            else if ((value & 0x40) != 0 && playerControl != null)
            {
                UserConsolePairs.Add(new Tuple<byte, byte>(playerControl.PlayerId, consoleId));
                if (UserCount >= 2)
                {
                    Countdown = 10000f;
                }
            }
            else if ((value & 0x20) != 0 && playerControl != null)
            {
                UserConsolePairs.Remove(new Tuple<byte, byte>(playerControl.PlayerId, consoleId));
            }
        }

        public bool GetConsoleComplete(int consoleId)
        {
            return UserConsolePairs.Any(x => x.Item2 == consoleId);
        }
    }
}
