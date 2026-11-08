using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SecurityCameraSystemType : ISystemType
    {
        private readonly HashSet<byte> _playersUsing = new();

        public bool InUse => _playersUsing.Count > 0;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.WritePacked(_playersUsing.Count);
            foreach (var playerId in _playersUsing)
            {
                writer.Write(playerId);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            _playersUsing.Clear();
            var num = reader.ReadPackedInt32();
            for (var i = 0; i < num; i++)
            {
                _playersUsing.Add(reader.ReadByte());
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            if (playerControl == null)
            {
                return;
            }

            if (reader.ReadByte() == 1)
            {
                _playersUsing.Add(playerControl.PlayerId);
            }
            else
            {
                _playersUsing.Remove(playerControl.PlayerId);
            }
        }
    }
}
