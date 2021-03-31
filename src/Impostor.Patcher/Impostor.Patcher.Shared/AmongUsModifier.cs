using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Impostor.Patcher.Shared.Events;
using Impostor.Patcher.Shared.Innersloth;
using ErrorEventArgs = Impostor.Patcher.Shared.Events.ErrorEventArgs;

namespace Impostor.Patcher.Shared
{
    public class AmongUsModifier
    {
        private const uint AppId = 945360;
        public const string DefaultRegionName = "Impostor";
        public const ushort DefaultPort = 22023;

        private readonly string _amongUsDir;
        private readonly string _regionFile;

        public string RegionName { get; set; } = DefaultRegionName;

        public AmongUsModifier()
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow");

            if (!Directory.Exists(appData))
            {
                appData = FindProtonAppData();
            }

            if (appData == null)
                return;

            _amongUsDir = Path.Combine(appData, "Innersloth", "Among Us");
            _regionFile = Path.Combine(_amongUsDir, "regionInfo.dat");
        }

        private string FindProtonAppData()
        {
            string steamApps;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                steamApps = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam", "steam", "steamapps");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                steamApps = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam", "steamapps");
            }
            else
            {
                return null;
            }

            if (!Directory.Exists(steamApps))
                return null;

            var libraries = new List<string>
            {
                steamApps,
            };

            var vdf = Path.Combine(steamApps, "libraryfolders.vdf");
            if (File.Exists(vdf))
            {
                var libraryFolders = VdfConvert.Deserialize(File.ReadAllText(vdf));

                foreach (var libraryFolder in libraryFolders.Value.Children<VProperty>())
                {
                    if (!int.TryParse(libraryFolder.Key, out _))
                        continue;

                    libraries.Add(Path.Combine(libraryFolder.Value.Value<string>(), "steamapps"));
                }
            }

            foreach (var library in libraries)
            {
                var path = Path.Combine(library, "compatdata", AppId.ToString(), "pfx", "drive_c", "users", "steamuser", "AppData", "LocalLow");
                if (Directory.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        public async Task<bool> SaveIpAsync(string input)
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
                        return false;
                    }

                    // Use first IPv4 result.
                    ipAddress = hostAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (SocketException)
                {
                    OnError("Failed to resolve hostname.");
                    return false;
                }
            }

            // Only IPv4.
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                OnError("Invalid IP Address entered, only IPv4 is allowed.");
                return false;
            }

            var result = WriteIp(ipAddress, port);
            OnSaved(ip, port);

            return result;
        }

        /// <summary>
        ///     Writes an IP Address to the Among Us region file.
        /// </summary>
        /// <param name="ipAddress">The IPv4 address to write.</param>
        /// <param name="port"></param>
        private bool WriteIp(IPAddress ipAddress, ushort port)
        {
            if (ipAddress == null ||
                ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException(nameof(ipAddress));
            }

            if (!Directory.Exists(_amongUsDir))
            {
                OnError("Among Us directory was not found, is it installed? Try running it once.");
                return false;
            }

            using (var file = File.Open(_regionFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(file))
            {
                var ip = ipAddress.ToString();
                var region = new RegionInfo(RegionName, ip, new[]
                {
                    new ServerInfo($"{RegionName}-Master-1", ip, port),
                });

                region.Serialize(writer);

                return true;
            }
        }

        /// <summary>
        ///     Loads the existing region info from the Among Us.
        /// </summary>
        public bool TryLoadRegionInfo(out RegionInfo regionInfo)
        {
            regionInfo = null;

            if (!File.Exists(_regionFile))
            {
                return false;
            }

            using (var file = File.Open(_regionFile, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(file))
            {
                try
                {
                    regionInfo = RegionInfo.Deserialize(reader);
                    return true;
                }
                catch (Exception exception)
                {
                    OnError("Couldn't parse region info\n" + exception);
                    return false;
                }
            }
        }

        /// <summary>
        ///     Loads the existing IP Address from the Among Us region file
        ///     if it was set by Impostor before.
        /// </summary>
        public bool TryLoadIp(out string ipAddress)
        {
            ipAddress = null;

            if (!TryLoadRegionInfo(out var regionInfo))
            {
                return false;
            }

            if ((regionInfo.Name == RegionName || regionInfo.Name == DefaultRegionName) && regionInfo.Servers.Count >= 1)
            {
                ipAddress = regionInfo.Servers.ElementAt(0).Ip;
                return true;
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
