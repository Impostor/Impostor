using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo : ITaskInfo
        {
            private readonly Game _game;
            private readonly IInnerPlayerControl _player;

            public TaskInfo(Game game, IInnerPlayerControl player)
            {
                _game = game;
                _player = player;
            }

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
