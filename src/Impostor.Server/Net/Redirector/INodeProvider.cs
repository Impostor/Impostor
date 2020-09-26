using System.Net;

namespace Impostor.Server.Net.Redirector
{
    internal interface INodeProvider
    {
        IPEndPoint Get();
        IPEndPoint Find(string gameCode);
        void Save(string gameCode, IPEndPoint endPoint);
        void Remove(string gameCode);
    }
}