using System.Collections.Generic;

namespace Impostor.Server.Config
{
    public class ServerRedirectorConfig
    {
        public const string Section = "ServerRedirector";

        public bool Enabled { get; set; }

        public bool Master { get; set; }

        public NodeLocator Locator { get; set; }

        public List<ServerRedirectorNode> Nodes { get; set; }

        public class NodeLocator
        {
            public string Redis { get; set; }

            public string UdpMasterEndpoint { get; set; }
        }
    }
}