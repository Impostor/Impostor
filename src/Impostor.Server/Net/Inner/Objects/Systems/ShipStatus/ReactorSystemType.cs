using System;
using System.Collections.Generic;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class ReactorSystemType : ISystemType, IActivatable
    {
        public ReactorSystemType()
        {
            Countdown = 10000f;
            UserConsolePairs = new HashSet<Tuple<byte, byte>>();
        }

        public float Countdown { get; private set; }

        public HashSet<Tuple<byte, byte>> UserConsolePairs { get; }

        public bool IsActive => Countdown < 10000.0;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();
            UserConsolePairs.Clear(); // TODO: Thread safety

            var count = reader.ReadPackedInt32();

            for (var i = 0; i < count; i++)
            {
                UserConsolePairs.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
            }
        }
    }
}
