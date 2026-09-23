using System.Collections.Generic;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class MedScanSystem : ISystemType
    {
        public MedScanSystem()
        {
            UsersList = new List<byte>();
        }

        public List<byte> UsersList { get; }

        public byte CurrentUser { get; private set; } = byte.MaxValue;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.WritePacked(UsersList.Count);
            foreach (var user in UsersList)
            {
                writer.Write(user);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            UsersList.Clear();

            var num = reader.ReadPackedInt32();

            for (var i = 0; i < num; i++)
            {
                UsersList.Add(reader.ReadByte());
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            var value = reader.ReadByte();
            var playerId = (byte)(value & 0x1f);
            if ((value & 0x80) != 0)
            {
                if (!UsersList.Contains(playerId))
                {
                    UsersList.Add(playerId);
                }
            }
            else if ((value & 0x40) != 0)
            {
                UsersList.RemoveAll(x => x == playerId);
                if (CurrentUser == playerId)
                {
                    CurrentUser = byte.MaxValue;
                }
            }
        }
    }
}
