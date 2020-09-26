using System;
using System.Net;

namespace Impostor.Server.Net.Redirector
{
    public class NodeProviderNoOp : INodeProvider
    {
        public IPEndPoint Get()
        {
            throw new NotImplementedException();
        }

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