namespace Impostor.Server.Data
{
    public class ServerConfig
    {
        public string PublicIp { get; set; }
        public ushort PublicPort { get; set; }
        public string ListenIp { get; set; }
        public ushort ListenPort { get; set; }
    }
}