using System.Net;
using System.Text.Json.Serialization;

namespace SelfHttpMatchmaker.Types;

public class HostServer
{
    [JsonPropertyName("Ip")] public required long Ip { get; init; }

    [JsonPropertyName("Port")] public required ushort Port { get; init; }

    public static HostServer From(IPAddress ipAddress, ushort port)
    {
        return new HostServer
        {
#pragma warning disable CS0618 // 类型或成员已过时
            Ip = ipAddress.Address,
#pragma warning restore CS0618 // 类型或成员已过时
            Port = port,
        };
    }
}
