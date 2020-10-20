using System.Net;
using System.Threading.Tasks;

namespace Impostor.Server.Net.Redirector
{
    public interface INodeLocator
    {
        IPEndPoint Find(string gameCode);

        void Save(string gameCode, IPEndPoint endPoint);

        void Remove(string gameCode);
    }

    public interface IAsyncNodeLocator
    {
        ValueTask<IPEndPoint> FindAsync(string gameCode);

        ValueTask SaveAsync(string gameCode, IPEndPoint endPoint);

        ValueTask RemoveAsync(string gameCode);
    }
}