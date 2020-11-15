using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public class TaskInfo : ITaskInfo
        {
            public TaskTypes Type { get; internal set; }

            public bool Complete { get; internal set; }

            public void Serialize(IMessageWriter writer)
            {
                writer.WritePacked((uint)Type);
                writer.Write(Complete);
            }

            public void Deserialize(IMessageReader reader)
            {
                this.Type = (TaskTypes)reader.ReadPackedUInt32();
                this.Complete = reader.ReadBoolean();
            }
        }
    }
}