using System;
using System.Collections.Generic;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData.Objects
{
    public partial class InnerGameData
    {
        public class PlayerInfo
        {
            public PlayerInfo(byte playerId)
            {
                PlayerId = playerId;
            }

            public byte PlayerId { get; }

            public string PlayerName { get; private set; }

            public byte ColorId { get; private set; }

            public uint HatId { get; private set; }

            public uint SkinId { get; private set; }

            public bool Disconnected { get; private set; }

            public bool IsImpostor { get; private set; }

            public bool IsDead { get; private set; }

            public List<TaskInfo> Tasks { get; private set; }

            public void Serialize(IMessageWriter writer)
            {
                throw new NotImplementedException();
            }

            public void Deserialize(IMessageReader reader)
            {
                PlayerName = reader.ReadString();
                ColorId = reader.ReadByte();
                HatId = reader.ReadPackedUInt32();
                SkinId = reader.ReadPackedUInt32();
                var flag = reader.ReadByte();
                Disconnected = (flag & 1) > 0;
                IsImpostor = (flag & 2) > 0;
                IsDead = (flag & 4) > 0;
                var taskCount = reader.ReadByte();
                Tasks = new List<TaskInfo>();
                for (var i = 0; i < taskCount; i++)
                {
                    Tasks.Add(new TaskInfo());
                    Tasks[i].Deserialize(reader);
                }
            }
        }
    }
}