using System.Net;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorNoOp : INodeLocator
    {
        public IPEndPoint Find(string gameCode)
        {
            // Do nothing.
            return null;
        }

        public void Save(string gameCode, IPEndPoint endPoint)
        {
            // Do nothing.
        }

        public void Remove(string gameCode)
        {
            // Do nothing.
        }
    }
}