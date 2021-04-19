using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo : ITaskInfo
        {
            public async ValueTask CompleteAsync()
            {
                if (Complete)
                {
                    return;
                }

                Complete = true;

                // Send RPC.
                using var writer = _game.StartRpc(_player.NetId, RpcCalls.Exiled);
                Rpc01CompleteTask.Serialize(writer, Id);
                await _game.FinishRpcAsync(writer);

                // Notify plugins.
                await _eventManager.CallAsync(new PlayerCompletedTaskEvent(_game, _game.GetClientPlayer(_player.OwnerId), _player, this));
            }
        }
    }
}
