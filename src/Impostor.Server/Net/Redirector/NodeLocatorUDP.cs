using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Impostor.Server.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorUdp : INodeLocator, IDisposable
    {
        private readonly ILogger<NodeLocatorUdp> _logger;
        private readonly bool _isMaster;
        private readonly IPEndPoint? _server;
        private readonly UdpClient? _client;
        private readonly ConcurrentDictionary<string, AvailableNode>? _availableNodes;

        public NodeLocatorUdp(ILogger<NodeLocatorUdp> logger, IOptions<ServerRedirectorConfig> config)
        {
            _logger = logger;

            if (config.Value.Master)
            {
                _isMaster = true;
                _availableNodes = new ConcurrentDictionary<string, AvailableNode>();
            }
            else
            {
                _isMaster = false;

                if (!IPEndPoint.TryParse(config.Value.Locator!.UdpMasterEndpoint, out var endpoint))
                {
                    throw new ArgumentException("UdpMasterEndpoint should be in the ip:port format.");
                }

                _logger.LogWarning("Node server will send updates to {0}.", endpoint);
                _server = endpoint;
                _client = new UdpClient();

                try
                {
                    _client.DontFragment = true;
                }
                catch (SocketException)
                {
                }
            }
        }

        public void Update(IPEndPoint ip, string gameCode)
        {
            _logger.LogDebug("Received update {0} -> {1}", gameCode, ip);

            _availableNodes!.AddOrUpdate(
                gameCode,
                s => new AvailableNode(ip, DateTimeOffset.UtcNow),
                (s, node) =>
                {
                    node.Endpoint = ip;
                    node.LastUpdated = DateTimeOffset.UtcNow;

                    return node;
                });

            foreach (var (key, value) in _availableNodes)
            {
                if (value.Expired)
                {
                    _availableNodes.TryRemove(key, out _);
                }
            }
        }

        public ValueTask<IPEndPoint?> FindAsync(string gameCode)
        {
            if (!_isMaster)
            {
                return ValueTask.FromResult(default(IPEndPoint));
            }

            if (_availableNodes!.TryGetValue(gameCode, out var node))
            {
                if (node.Expired)
                {
                    _availableNodes.TryRemove(gameCode, out _);
                    return ValueTask.FromResult(default(IPEndPoint));
                }

                return ValueTask.FromResult(node.Endpoint)!;
            }

            return ValueTask.FromResult(default(IPEndPoint));
        }

        public ValueTask RemoveAsync(string gameCode)
        {
            if (!_isMaster)
            {
                return ValueTask.CompletedTask;
            }

            _availableNodes!.TryRemove(gameCode, out _);
            return ValueTask.CompletedTask;
        }

        public ValueTask SaveAsync(string gameCode, IPEndPoint endPoint)
        {
            var data = Encoding.UTF8.GetBytes($"{gameCode},{endPoint}");
            _client!.Send(data, data.Length, _server);
            return ValueTask.CompletedTask;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private class AvailableNode
        {
            public AvailableNode(IPEndPoint endpoint, DateTimeOffset lastUpdated)
            {
                Endpoint = endpoint;
                LastUpdated = lastUpdated;
            }

            public IPEndPoint Endpoint { get; set; }

            public DateTimeOffset LastUpdated { get; set; }

            public bool Expired => LastUpdated < DateTimeOffset.UtcNow.AddHours(-1);
        }
    }
}
