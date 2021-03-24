using System.Text.Json.Serialization;

namespace Impostor.Server.Config
{
    public class ServerRedirectorNode
    {
        [JsonConstructor]
        public ServerRedirectorNode(string ip, ushort port)
        {
            Ip = ip;
            Port = port;
        }

        public string Ip { get; }

        public ushort Port { get; }
    }
}
