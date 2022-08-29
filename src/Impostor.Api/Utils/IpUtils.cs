using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Impostor.Api.Utils
{
    internal static class IpUtils
    {
        public static string ResolveIp(string ip)
        {
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

                    // Use first IPv4 result.
                    ipAddress = hostAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);
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

            return ipAddress.ToString();
        }
    }
}
