using System.Net;
using System.Threading.Tasks;

namespace Impostor.Server.Net.Redirector
{
    public interface INodeLocator
    {
        ValueTask<IPEndPoint?> FindAsync(string gameCode);

        ValueTask SaveAsync(string gameCode, IPEndPoint endPoint);

        ValueTask RemoveAsync(string gameCode);
    }
}
