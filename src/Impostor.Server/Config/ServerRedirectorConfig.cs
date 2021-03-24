using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Impostor.Server.Config
{
    public class ServerRedirectorConfig
    {
        public const string Section = "ServerRedirector";

        public bool Enabled { get; set; }

        public bool Master { get; set; }

        public NodeLocator? Locator { get; set; }

        public List<ServerRedirectorNode>? Nodes { get; set; }

        public class NodeLocator
        {
            [AllowNull]
            public string Redis { get; set; }

            [AllowNull]
            public string UdpMasterEndpoint { get; set; }
        }
    }
}
