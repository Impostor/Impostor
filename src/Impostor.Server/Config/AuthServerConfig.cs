using System.IO;
using Impostor.Server.Utils;

namespace Impostor.Server.Config
{
    internal class AuthServerConfig
    {
        public const string Section = "AuthServer";

        private string? _resolvedListenIp;

        public bool Enabled { get; set; } = false;

        public string ListenIp { get; set; } = "0.0.0.0";

        public ushort ListenPort { get; set; } = 22025;

        public string Certificate { get; set; } = Path.Combine("dtls", "certificate.pem");

        public string PrivateKey { get; set; } = Path.Combine("dtls", "key.pem");

        public string ResolveListenIp()
        {
            return _resolvedListenIp ??= IpUtils.ResolveIp(ListenIp);
        }
    }
}
