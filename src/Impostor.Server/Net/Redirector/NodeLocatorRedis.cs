using System;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorRedis : INodeLocator
    {
        private readonly IDistributedCache _cache;

        public NodeLocatorRedis(ILogger<NodeLocatorRedis> logger, IDistributedCache cache)
        {
            logger.LogWarning("Using the redis NodeLocator.");
            _cache = cache;
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
                SlidingExpiration = TimeSpan.FromHours(1),
            });
        }

        public void Remove(string gameCode)
        {
            _cache.Remove(gameCode);
        }
    }
}