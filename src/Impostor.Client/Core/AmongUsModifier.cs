using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Impostor.Client.Core.Events;
using Impostor.Client.Forms;
using Impostor.Shared.Innersloth;
using ErrorEventArgs = Impostor.Client.Core.Events.ErrorEventArgs;

namespace Impostor.Client.Core
{
    public class AmongUsModifier
    {
        private const string RegionName = "Impostor";
        public const ushort DefaultPort = 22023;

        private readonly FrmMain _mainForm;
        
        public AmongUsModifier(FrmMain mainForm)
        {
            _mainForm = mainForm;
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
                        OnError("Invalid IP Address entered", false);
                        return;
                    }
                    
                    // Use first IPv4 result.
                    ipAddress = hostAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (SocketException)
                {
                    OnError("Failed to resolve hostname.", false);
                    return;
                }
            }
            
            // Only IPv4.
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                OnError("Invalid IP Address entered, only IPv4 is allowed.", false);
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
            
            if (!Directory.Exists(_mainForm.Configuration.InstallFolder))
            {
                OnError("Among Us directory wasn't found, is it installed? Try running it once ; otherwise the folder should be located in \"LocalLow\\Innersloth\\Among Us\\\".\n\rClicking \"Yes\" will prompt you to chose another folder to use as installation folder, \"No\" will open the LocalLow folder, and \"Cancel\" will basically cancel and return to main program.", true);
                return;
            }
            
            using (var file = File.Open(Path.Combine(_mainForm.Configuration.InstallFolder, "regionInfo.dat"), FileMode.Create, FileAccess.Write))
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
            
            if (!File.Exists(Path.Combine(_mainForm.Configuration.InstallFolder, "regionInfo.dat")))
            {
                return false;
            }

            using (var file = File.Open(Path.Combine(_mainForm.Configuration.InstallFolder, "regionInfo.dat"), FileMode.Open, FileAccess.Read))
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

        private void OnError(string message, bool openLocalLow)
        {
            Error?.Invoke(this, new ErrorEventArgs(message, openLocalLow));
        }

        private void OnSaved(string ipAddress, ushort port)
        {
            Saved?.Invoke(this, new SavedEventArgs(ipAddress, port));
        }
            
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<SavedEventArgs> Saved;
    }
}