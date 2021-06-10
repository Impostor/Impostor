using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo
        {
            public async ValueTask CompleteAsync()
            {
                if (_playerInfo.Controller == null)
                {
                    throw new ImpostorException("Can't complete a task that doesn't have a player assigned");
                }

                var player = _playerInfo.Controller;

                if (Complete)
                {
                    throw new ImpostorException("Can't complete a task that is already completed");
                }

                Complete = true;

                // Send RPC.
                using var writer = player.Game.StartRpc(player.NetId, RpcCalls.CompleteTask);
                Rpc01CompleteTask.Serialize(writer, Id);
                await player.Game.FinishRpcAsync(writer);

                // Notify plugins.
                await _eventManager.CallAsync(new PlayerCompletedTaskEvent(player.Game, player.Game.GetClientPlayer(player.OwnerId)!, player, this));
            }
        }
    }
}
