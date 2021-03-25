using System.Diagnostics.CodeAnalysis;

namespace Impostor.Server.Config
{
    public class ServerRedirectorNode
    {
        [AllowNull]
        public string Ip { get; set; }

        [AllowNull]
        public ushort Port { get; set; }
    }
}
