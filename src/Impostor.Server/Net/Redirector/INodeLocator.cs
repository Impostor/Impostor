using System.Net;

namespace Impostor.Server.Net.Redirector
{
    public interface INodeLocator
    {
        IPEndPoint Find(string gameCode);

        void Save(string gameCode, IPEndPoint endPoint);

        void Remove(string gameCode);
    }
}