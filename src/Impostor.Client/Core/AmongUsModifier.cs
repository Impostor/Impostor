using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Impostor.Client.Core.Events;
using Impostor.Shared.Innersloth;
using ErrorEventArgs = Impostor.Client.Core.Events.ErrorEventArgs;

namespace Impostor.Client.Core
{
    public class AmongUsModifier
    {
        private const string RegionName = "Impostor";
        public const ushort DefaultPort = 22023;
        
        private readonly string _amongUsDir;
        private readonly string _regionFile;
        
        public AmongUsModifier()
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow");
            var amongUsDir = Path.Combine(appData, "Innersloth", "Among Us");

            _amongUsDir = amongUsDir;
            _regionFile = Path.Combine(amongUsDir, "regionInfo.dat");
        }
        
        public async Task SaveIp(string input)
        {
            // Filter out whitespace.
            input = input.Trim();
            
            // Split port from ip.
            // Only IPv4 is supported so just do it simple.
            var ip = string.Empty;
            var port = DefaultPort;
            
            var parts = input.Split(':');
            if (parts.Length >= 1)
            {
                ip = parts[0];
            }

            if (parts.Length >= 2)
            {
                ushort.TryParse(parts[1], out port);
            }
            
            // Check if a valid IP address was entered.
            if (!IPAddress.TryParse(ip, out var ipAddress))
            {
                // Attempt to resolve DNS.
                try
                {
                    var hostAddresses = await Dns.GetHostAddressesAsync(ip);
                    if (hostAddresses.Length == 0)
                    {
                        OnError("Invalid IP Address entered");
                        return;
                    }
                    
                    // Use first IPv4 result.
                    ipAddress = hostAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (SocketException)
                {
                    OnError("Failed to resolve hostname.");
                    return;
                }
            }
            
            // Only IPv4.
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                OnError("Invalid IP Address entered, only IPv4 is allowed.");
                return;
            }

            WriteIp(ipAddress, port);
        }

        /// <summary>
        ///     Writes an IP Address to the Among Us region file.
        /// </summary>
        /// <param name="ipAddress">The IPv4 address to write.</param>
        /// <param name="port"></param>
        private void WriteIp(IPAddress ipAddress, ushort port)
        {
            if (ipAddress == null || 
                ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException(nameof(ipAddress));
            }
            
            if (!Directory.Exists(_amongUsDir))
            {
                OnError("Among Us directory was not found, is it installed? Try running it once.");
                return;
            }
            
            using (var file = File.Open(_regionFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(file))
            {
                var ip = ipAddress.ToString();
                var region = new RegionInfo(RegionName, ip, new[]
                {
                    new ServerInfo($"{RegionName}-Master-1", ip, port)
                });
                    
                region.Serialize(writer);

                OnSaved(ip, port);
            }
        }

        /// <summary>
        ///     Loads the existing IP Address from the Among Us region file
        ///     if it was set by Impostor before.
        /// </summary>
        public bool TryLoadIp(out string ipAddress)
        {
            ipAddress = null;
            
            if (!File.Exists(_regionFile))
            {
                return false;
            }

            using (var file = File.Open(_regionFile, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(file))
            {
                var region = RegionInfo.Deserialize(reader);
                if (region.Name == RegionName && region.Servers.Count >= 1)
                {
                    ipAddress = region.Servers[0].Ip;
                    return true;
                }
            }

            return false;
        }

        private void OnError(string message)
        {
            Error?.Invoke(this, new ErrorEventArgs(message));
        }

        private void OnSaved(string ipAddress, ushort port)
        {
            Saved?.Invoke(this, new SavedEventArgs(ipAddress, port));
        }
            
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<SavedEventArgs> Saved;
    }
}