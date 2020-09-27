using System;
using System.Net;
using Agones;
using Microsoft.Extensions.Caching.Distributed;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorAgones : INodeLocator
    {
        private readonly AgonesSDK _agones;
        
        public NodeLocatorAgones(AgonesSDK agones)
        {
            _agones = agones;
        }

        public IPEndPoint Find(string gameCode)
        {
            // Do nothing.
            return null;
        }

        public void Save(string gameCode, IPEndPoint endPoint)
        {
            // do nothing
        }

        public void Remove(string gameCode)
        {
            // Do nothing.
        }
    }
}