using System.Collections.Generic;
using System.Net;
using Impostor.Server.Config;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Redirector
{
    internal class NodeProviderConfig : INodeProvider
    {
        private readonly List<IPEndPoint> _nodes;
        private readonly object _lock;
        private int _currentIndex;

        public NodeProviderConfig(IOptions<ServerRedirectorConfig> redirectorConfig)
        {
            _nodes = new List<IPEndPoint>();
            _lock = new object();

            if (redirectorConfig.Value.Nodes != null)
            {
                foreach (var node in redirectorConfig.Value.Nodes)
                {
                    _nodes.Add(new IPEndPoint(IPAddress.Parse(node.Ip), node.Port));
                }
            }
        }

        public IPEndPoint Get()
        {
            lock (_lock)
            {
                var node = _nodes[_currentIndex++];

                if (_currentIndex == _nodes.Count)
                {
                    _currentIndex = 0;
                }

                return node;
            }
        }
    }
}
