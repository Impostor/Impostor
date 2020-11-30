using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public class TaskInfo : ITaskInfo
        {
            public uint Id { get; internal set; }

            public bool Complete { get; internal set; }

            public TaskTypes Type { get; internal set; }

            public void Serialize(IMessageWriter writer)
            {
                writer.WritePacked((uint)Id);
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
