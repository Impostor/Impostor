using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Impostor.Client.Core
{
    public class Configuration
    {
        private const string FileAmongUsFolder = @"install_folder.txt";

        private const string FileRecentIps = @"recent_ips.txt";
        private const int MaxRecentIps = 5;

        private readonly string _baseDir;
        private readonly string _folderConfigPath;
        private readonly string _defaultInstallationFolder;
        private string _installFolder;
        private string _currInstallFolder;
        private readonly string _recentIpsPath;
        private readonly List<string> _recentIps;

        public Configuration(string defaultInstallationFolder)
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            _baseDir = Path.Combine(appData, "Impostor");

            _folderConfigPath = Path.Combine(_baseDir, FileAmongUsFolder);
            _defaultInstallationFolder = defaultInstallationFolder;

            _recentIpsPath = Path.Combine(_baseDir, FileRecentIps);
            _recentIps = new List<string>();
        }

        public string InstallFolder => _currInstallFolder;

        public IReadOnlyList<string> RecentIps => _recentIps;

        public void Load()
        {
            if (File.Exists(_folderConfigPath))
            {
                _currInstallFolder = File.ReadAllLines(_folderConfigPath)[0];
            }
            else
            {
                File.WriteAllLines(_folderConfigPath, new string[] { _defaultInstallationFolder });
                Load();
                return;
            }

            if (File.Exists(_recentIpsPath))
            {
                _recentIps.AddRange(File.ReadAllLines(_recentIpsPath));
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(_baseDir);

            if (!Directory.Exists(_baseDir))
            {
                return;
            }

            if (_recentIps.Count > 0)
            {
                File.WriteAllLines(_recentIpsPath, _recentIps);
            }

            if (_installFolder != _currInstallFolder)
            {
                File.WriteAllLines(_folderConfigPath, new string[] { (_installFolder ?? _defaultInstallationFolder) });
            }
        }

        public void AddIp(string ip)
        {
            if (_recentIps.Contains(ip))
            {
                _recentIps.Remove(ip);
            }

            _recentIps.Insert(0, ip);

            if (_recentIps.Count > MaxRecentIps)
            {
                _recentIps.RemoveAt(MaxRecentIps);
            }
        }

        public void SetAmongUsFolder(string path)
        {
            _installFolder = path;
            MessageBox.Show($"Among Us installation folder set as {path}.", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
