using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects;

internal partial class InnerGameData
{
    public partial class TaskInfo(InnerPlayerInfo playerInfo, IEventManager eventManager, uint id, TaskData? task)
        : ITaskInfo
    {
        public uint Id { get; internal set; } = id;

        public TaskData? Task { get; internal set; } = task;

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
