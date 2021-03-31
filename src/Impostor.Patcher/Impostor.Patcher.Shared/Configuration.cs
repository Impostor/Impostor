using System;
using System.Collections.Generic;
using System.IO;

namespace Impostor.Patcher.Shared
{
    public class Configuration
    {
        private const string FileRecentIps = @"recent_ips.txt";
        private const int MaxRecentIps = 5;

        private readonly string _baseDir;
        private readonly string _recentIpsPath;
        private readonly List<string> _recentIps;

        public Configuration()
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            _baseDir = Path.Combine(appData, "Impostor");
            _recentIpsPath = Path.Combine(_baseDir, FileRecentIps);
            _recentIps = new List<string>();
        }

        public IReadOnlyList<string> RecentIps => _recentIps;

        public void Load()
        {
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
    }
}
