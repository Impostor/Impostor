using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData.Objects
{
    public partial class InnerGameData
    {
        public class TaskInfo
        {
            public uint Id { get; private set; }

            public bool Complete { get; private set; }

            public void Serialize(IMessageWriter writer)
            {
                writer.WritePacked(Id);
                writer.Write(Complete);
            }

            public void Deserialize(IMessageReader reader)
            {
                this.Id = reader.ReadPackedUInt32();
                this.Complete = reader.ReadBoolean();
            }
        }
    }
}