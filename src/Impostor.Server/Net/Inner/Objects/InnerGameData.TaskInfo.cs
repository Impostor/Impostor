using Impostor.Api.Events.Managers;
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
            private readonly IEventManager _eventManager;
            private readonly Game _game;
            private readonly IInnerPlayerControl _player;

            public TaskInfo(IEventManager eventManager, Game game, IInnerPlayerControl player)
            {
                _eventManager = eventManager;
                _game = game;
                _player = player;
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
