using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorRedis : INodeLocator, IAsyncNodeLocator
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

        public async ValueTask<IPEndPoint> FindAsync(string gameCode)
        {
            var entry = await _cache.GetStringAsync(gameCode);
            if (entry == null)
            {
                return null;
            }

            return IPEndPoint.Parse(entry);
        }

        public async ValueTask SaveAsync(string gameCode, IPEndPoint endPoint)
        {
            await _cache.SetStringAsync(gameCode, endPoint.ToString(), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1),
            });
        }


        public async ValueTask RemoveAsync(string gameCode)
        {
            await _cache.RemoveAsync(gameCode);
        }
    }
}