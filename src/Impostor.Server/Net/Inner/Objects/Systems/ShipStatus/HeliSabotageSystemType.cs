using System;
using System.Collections.Generic;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class HeliSabotageSystemType : ISystemType, IActivatable
    {
        public HeliSabotageSystemType()
        {
            Countdown = 10000f;
            ActiveConsoles = new HashSet<Tuple<byte, byte>>();
            CompletedConsoles = new HashSet<byte>();
        }

        public float Countdown { get; private set; }

        public float Timer { get; private set; }

        public HashSet<Tuple<byte, byte>> ActiveConsoles { get; }

        public HashSet<byte> CompletedConsoles { get; }

        public bool IsActive => Countdown < 10000.0;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();
            Timer = reader.ReadSingle();
            ActiveConsoles.Clear(); // TODO: Thread safety
            CompletedConsoles.Clear(); // TODO: Thread safety

            var activeCount = reader.ReadPackedUInt32();

            for (var i = 0; i < activeCount; i++)
            {
                ActiveConsoles.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
            }

            var completedCount = reader.ReadPackedUInt32();

            for (var i = 0; i < completedCount; i++)
            {
                CompletedConsoles.Add(reader.ReadByte());
            }
        }
    }
}
