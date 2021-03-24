using System.Text.Json.Serialization;
using Impostor.Server.Utils;

namespace Impostor.Server.Config
{
    internal class AnnouncementsServerConfig
    {
        public const string Section = "AnnouncementsServer";

        private string? _resolvedListenIp;

        [JsonConstructor]
        public AnnouncementsServerConfig(bool enabled = true, string listenIp = "0.0.0.0", ushort listenPort = 22024)
        {
            Enabled = enabled;
            ListenIp = listenIp;
            ListenPort = listenPort;
        }

        public bool Enabled { get; }

        public string ListenIp { get; }

        public ushort ListenPort { get; }

        public string ResolveListenIp()
        {
            return _resolvedListenIp ??= IpUtils.ResolveIp(ListenIp);
        }
    }
}
