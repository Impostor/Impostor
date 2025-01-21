using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Impostor.Api.Utils;

internal static class IpUtils
{
    private static readonly Dictionary<string, string> CacheResolveIp = new();

    public static string ResolveIp(this string ip)
    {
        if (CacheResolveIp.TryGetValue(ip, out var value))
        {
            return value;
        }

        // Check if valid ip was entered.
        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            // Attempt to resolve DNS.
            try
            {
                var hostAddresses = Dns.GetHostAddresses(ip);
                if (hostAddresses.Length == 0)
                {
                    throw new ImpostorConfigException($"Invalid IP Address entered '{ip}'.");
                }

                // Use first result.
                ipAddress = hostAddresses.First(x => x.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6);
            }
            catch (SocketException)
            {
                throw new ImpostorConfigException($"Failed to resolve hostname '{ip}'.");
            }
        }

        // Only IPv4.
        if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
        {
            throw new ImpostorConfigException($"Invalid IP Address entered '{ipAddress}', only IPv4 is supported by Among Us.");
        }

        return CacheResolveIp[ip] = ipAddress.ToString();
    }
}
