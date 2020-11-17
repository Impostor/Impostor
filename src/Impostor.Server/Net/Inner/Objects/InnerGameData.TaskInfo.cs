using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public class TaskInfo : ITaskInfo
        {
            public uint TaskIndex { get; internal set; }

            public TaskTypes Type { get; internal set; }

            public bool Complete { get; internal set; }

            public void Serialize(IMessageWriter writer)
            {
                writer.WritePacked((uint)Type);
                writer.Write(Complete);
            }

            public void Deserialize(IMessageReader reader)
            {
                TaskIndex = reader.ReadPackedUInt32();
                this.Complete = reader.ReadBoolean();
            }
        }
    }
}
