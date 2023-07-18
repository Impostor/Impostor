using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Manager
{
    internal class CompatibilityManager : ICompatibilityManager
    {
        private static readonly CompatData[] DefaultSupportedVersions =
        {
            new(GameVersion.GetVersion(2022, 11, 1), GameVersion.GetVersion(2022, 11, 1), true), // 2022.12.8

            new(GameVersion.GetVersion(2022, 11, 9), GameVersion.GetVersion(2022, 11, 9), true), // 2022.12.14

            new(GameVersion.GetVersion(2022, 12, 2), GameVersion.GetVersion(2022, 12, 2), true), // 2023.2.28

            new(GameVersion.GetVersion(2023, 1, 11), GameVersion.GetVersion(2023, 1, 11), true), // 2023.3.28s
            new(GameVersion.GetVersion(2023, 3, 13), GameVersion.GetVersion(2023, 1, 11), true), // 2023.3.28a
            new(GameVersion.GetVersion(2023, 4, 21), GameVersion.GetVersion(2023, 1, 11), true), // 2023.6.13

            new(GameVersion.GetVersion(2023, 5, 20), GameVersion.GetVersion(2023, 5, 20), true), // 2023.7.11
            new(GameVersion.GetVersion(2222, 0, 0), GameVersion.GetVersion(2023, 5, 20), false), // 2023.7.11 for host-only mods
        };

        // Map from client version to compatibility group
        private readonly Dictionary<int, int> _supportMap = new Dictionary<int, int>();
        private readonly ILogger<CompatibilityManager> _logger;
        private readonly bool initializationFinished;
        private int _lowestVersionSupported = int.MaxValue;
        private int _highestVersionSupported = 0;

        public CompatibilityManager(ILogger<CompatibilityManager> logger)
        {
            _logger = logger;
            initializationFinished = false;
            foreach (var compatData in DefaultSupportedVersions)
            {
                AddSupportedVersion(compatData.GameVersion, compatData.CompatGroup, compatData.IncludeInSupportRange);
            }

            ModifiedByUser = false;
            initializationFinished = true;
        }

        internal bool ModifiedByUser { get; private set; }

        public ICompatibilityManager.VersionCompareResult TryGetCompatibilityGroup(int clientVersion, out int? compatGroup)
        {
            compatGroup = null;
            if (_supportMap.TryGetValue(clientVersion, out var compat))
            {
                compatGroup = compat;
                return ICompatibilityManager.VersionCompareResult.Compatible;
            }
            else if (clientVersion < _lowestVersionSupported)
            {
                return ICompatibilityManager.VersionCompareResult.ClientTooOld;
            }
            else if (clientVersion > _highestVersionSupported)
            {
                return ICompatibilityManager.VersionCompareResult.ServerTooOld;
            }
            else
            {
                return ICompatibilityManager.VersionCompareResult.Unknown;
            }
        }

        public void AddSupportedVersion(int gameVersion, int compatGroup, bool includeInSupportRange)
        {
            if (initializationFinished)
            {
                ModifiedByUser = true;
                _logger.LogWarning("AddSupportedVersion was called by a plugin, this can create unexpected issues. Please proceed carefully");
            }

            _supportMap[gameVersion] = compatGroup;
            if (includeInSupportRange)
            {
                if (gameVersion < _lowestVersionSupported)
                {
                    _lowestVersionSupported = gameVersion;
                }

                if (gameVersion > _highestVersionSupported)
                {
                    _highestVersionSupported = gameVersion;
                }
            }
        }

        public bool RemoveSupportedVersion(int removedVersion)
        {
            return _supportMap.Remove(removedVersion);
        }

        internal record CompatData(int GameVersion, int CompatGroup, bool IncludeInSupportRange);
    }
}
