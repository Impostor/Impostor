using System;
using System.Collections.Generic;
using System.Net;
using Impostor.Server.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Redirector
{
    internal class NodeProviderRedis : INodeProvider
    {
        private readonly IDistributedCache _cache;
        private readonly List<IPEndPoint> _nodes;
        private readonly object _lock;
        private int _currentIndex;
        
        public NodeProviderRedis(IOptions<ServerRedirectorConfig> redirectorConfig, IDistributedCache cache)
        {
            _cache = cache;
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

        public IPEndPoint Find(string gameCode)
        {
            var entry = _cache.GetString(gameCode);
            if (entry == null)
            {
                return null;
            }
            
            return IPEndPoint.Parse(entry);
        }

        public void Save(string gameCode, IPEndPoint endPoint)
        {
            _cache.SetString(gameCode, endPoint.ToString(), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            });
        }

        public void Remove(string gameCode)
        {
            _cache.Remove(gameCode);
        }
    }
}