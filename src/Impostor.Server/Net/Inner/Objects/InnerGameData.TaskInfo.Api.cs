using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo : ITaskInfo
        {
            public async ValueTask SetCompleteAsync()
            {
                if (Complete)
                {
                    return;
                }

                Complete = true;

                using var writer = _game.StartRpc(_player.NetId, RpcCalls.CompleteTask);
                writer.Write(_player.PlayerInfo.Tasks.ToList().IndexOf(this));
                await _game.FinishRpcAsync(writer);
            }
        }
    }
}
