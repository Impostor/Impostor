using System.Collections.Generic;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SecurityCameraSystemType : ISystemType
    {
        public (HashSet<byte> UsingNow, HashSet<byte> StartedWatching, HashSet<byte> StoppedWatching) Players { get; internal set; } = (new (), new (), new ());

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            var count = reader.ReadPackedInt32();
            var nowInUseBy = new HashSet<byte>();
            for (var i = 0; i < count; i++)
            {
                nowInUseBy.Add(reader.ReadByte());
            }

            var startedWatching = new HashSet<byte>(nowInUseBy);
            startedWatching.ExceptWith(Players.UsingNow);

            var stoppedWatching = Players.UsingNow;
            stoppedWatching.ExceptWith(nowInUseBy);

            Players = (nowInUseBy, startedWatching, stoppedWatching);
        }
    }
}