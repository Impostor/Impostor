using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo : ITaskInfo
        {
            private readonly InnerPlayerInfo _playerInfo;
            private readonly IEventManager _eventManager;

            public TaskInfo(InnerPlayerInfo playerInfo, IEventManager eventManager, uint id, ITask task)
            {
                _playerInfo = playerInfo;
                _eventManager = eventManager;
                Id = id;
                Task = task;
            }

            public uint Id { get; internal set; }

            public ITask Task { get; internal set; }

            public bool Complete { get; internal set; }

            public void Serialize(IMessageWriter writer)
            {
                writer.WritePacked(Id);
                writer.Write(Complete);
            }

            public void Deserialize(IMessageReader reader)
            {
                Id = reader.ReadPackedUInt32();
                Complete = reader.ReadBoolean();
            }
        }
    }
}
