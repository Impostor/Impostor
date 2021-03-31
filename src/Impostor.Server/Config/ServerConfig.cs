using Impostor.Server.Utils;

namespace Impostor.Server.Config
{
    internal class ServerConfig
    {
        public const string Section = "Server";

        private string? _resolvedPublicIp;
        private string? _resolvedListenIp;

        public string PublicIp { get; set; } = "127.0.0.1";

        public ushort PublicPort { get; set; } = 22023;

        public string ListenIp { get; set; } = "127.0.0.1";

        public ushort ListenPort { get; set; } = 22023;

        public string ResolvePublicIp()
        {
            return _resolvedPublicIp ??= IpUtils.ResolveIp(PublicIp);
        }

        public string ResolveListenIp()
        {
            return _resolvedListenIp ??= IpUtils.ResolveIp(ListenIp);
        }
    }
}
