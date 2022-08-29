using System.Diagnostics.CodeAnalysis;

namespace Impostor.Api.Config
{
    public class ServerRedirectorNode
    {
        [AllowNull]
        public string Ip { get; set; }

        [AllowNull]
        public ushort Port { get; set; }
    }
}
