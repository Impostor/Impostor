using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Impostor.Server.Config
{
    public class ServerRedirectorConfig
    {
        public const string Section = "ServerRedirector";

        [JsonConstructor]
        public ServerRedirectorConfig(bool enabled = false, bool master = false, NodeLocator? locator = null, List<ServerRedirectorNode>? nodes = null)
        {
            Enabled = enabled;
            Master = master;
            Locator = locator;
            Nodes = nodes;
        }

        public bool Enabled { get; }

        public bool Master { get; }

        public NodeLocator? Locator { get; }

        public List<ServerRedirectorNode>? Nodes { get; }

        public class NodeLocator
        {
            [JsonConstructor]
            public NodeLocator(string redis, string udpMasterEndpoint)
            {
                Redis = redis;
                UdpMasterEndpoint = udpMasterEndpoint;
            }

            public string Redis { get; }

            public string UdpMasterEndpoint { get; }
        }
    }
}
