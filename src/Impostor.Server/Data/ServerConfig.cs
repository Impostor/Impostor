namespace Impostor.Server.Data
{
    internal class ServerConfig
    {
        public const string Section = "Server";

        public string PublicIp { get; set; } = "127.0.0.1";

        public ushort PublicPort { get; set; } = 22023;

        public string ListenIp { get; set; } = "127.0.0.1";

        public ushort ListenPort { get; set; } = 22023;
    }
}