using System;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SabotageSystemType : ISystemType
    {
        private readonly IActivatable[] _specials;

        public SabotageSystemType(IActivatable[] specials)
        {
            _specials = specials;
        }

        public float Timer { get; set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            Timer = reader.ReadSingle();
        }
    }
}
