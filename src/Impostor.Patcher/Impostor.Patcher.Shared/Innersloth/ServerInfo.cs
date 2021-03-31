using System.IO;
using System.Net;

namespace Impostor.Patcher.Shared.Innersloth
{
    public class ServerInfo
    {
        public string Name { get; }
        public string Ip { get; }
        public ushort Port { get; }

        public ServerInfo(string name, string ip, ushort port)
        {
            Name = name;
            Ip = ip;
            Port = port;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(IPAddress.Parse(Ip).GetAddressBytes());
            writer.Write(Port);
            writer.Write(0);
        }

        public static ServerInfo Deserialize(BinaryReader reader)
        {
            var name = reader.ReadString();
            var ip = new IPAddress(reader.ReadBytes(4)).ToString();
            var port = reader.ReadUInt16();
            var unknown = reader.ReadInt32();

            return new ServerInfo(name, ip, port);
        }
    }
}
