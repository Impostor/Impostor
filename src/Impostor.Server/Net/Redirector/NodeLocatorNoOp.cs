using System.Net;
using System.Threading.Tasks;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorNoOp : INodeLocator
    {
        public ValueTask<IPEndPoint?> FindAsync(string gameCode) => ValueTask.FromResult(default(IPEndPoint));

        public ValueTask SaveAsync(string gameCode, IPEndPoint endPoint) => ValueTask.CompletedTask;

        public ValueTask RemoveAsync(string gameCode) => ValueTask.CompletedTask;
    }
}
