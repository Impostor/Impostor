using System.Collections.Generic;
using System.IO;

namespace Impostor.Patcher.Shared.Innersloth
{
    public class RegionInfo
    {
        public RegionInfo(string name, string ping, IReadOnlyList<ServerInfo> servers)
        {
            Name = name;
            Ping = ping;
            Servers = servers;
        }

        public string Name { get; }
        public string Ping { get; }
        public IReadOnlyList<ServerInfo> Servers { get; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(0);
            writer.Write(Name);
            writer.Write(Ping);
            writer.Write(Servers.Count);

            foreach (var server in Servers)
            {
                server.Serialize(writer);
            }
        }

        public static RegionInfo Deserialize(BinaryReader reader)
        {
            var unknown = reader.ReadInt32();
            var name = reader.ReadString();
            var ping = reader.ReadString();
            var servers = new List<ServerInfo>();
            var serverCount = reader.ReadInt32();

            for (var i = 0; i < serverCount; i++)
            {
                servers.Add(ServerInfo.Deserialize(reader));
            }

            return new RegionInfo(name, ping, servers);
        }
    }
}
