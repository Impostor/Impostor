using System.Collections.Generic;
using Impostor.Api.Games;
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
        private readonly Dictionary<int, int> _supportMap = new();
        private readonly ILogger<CompatibilityManager> _logger;
        private int _lowestVersionSupported = int.MaxValue;
        private int _highestVersionSupported = 0;

        public CompatibilityManager(ILogger<CompatibilityManager> logger)
        {
            _logger = logger;
            foreach (var compatData in DefaultSupportedVersions)
            {
                AddSupportedVersionInternal(compatData.GameVersion, compatData.CompatGroup, compatData.IncludeInSupportRange);
            }
        }

        public int? TryGetCompatibilityGroup(int clientVersion)
        {
            // Innersloth servers allow disabling server authority by setting the Patch field to 25.
            // We should allow crossplay between client versions with this field set and those without.
            if ((clientVersion % 50) >= 25)
            {
                clientVersion -= 25;
            }

            if (_supportMap.TryGetValue(clientVersion, out var compatGroup))
            {
                return compatGroup;
            }
            else
            {
                return null;
            }
        }

        public ICompatibilityManager.VersionCompareResult CanConnectToServer(int clientVersion)
        {
            if (this.TryGetCompatibilityGroup(clientVersion) != null)
            {
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

        public GameJoinError CanJoinGame(int hostVersion, int playerVersion)
        {
            if (hostVersion == playerVersion)
            {
                // Optimize a common case: a player on version X should always be able to join version X
                return GameJoinError.None;
            }

            var hostCompatGroup = this.TryGetCompatibilityGroup(hostVersion);
            var playerCompatGroup = this.TryGetCompatibilityGroup(playerVersion);

            if (hostCompatGroup == null || playerCompatGroup == null)
            {
                return GameJoinError.InvalidClient;
            }
            else if (playerCompatGroup < hostCompatGroup)
            {
                return GameJoinError.ClientOutdated;
            }
            else if (playerCompatGroup > hostCompatGroup)
            {
                return GameJoinError.ClientTooNew;
            }
            else
            {
                return GameJoinError.None;
            }
        }

        public void AddSupportedVersion(int gameVersion, int compatGroup)
        {
            _logger.LogWarning("AddSupportedVersion was called by a plugin, this can create unexpected issues. Please proceed carefully");

            AddSupportedVersionInternal(gameVersion, compatGroup, true);
        }

        private void AddSupportedVersionInternal(int gameVersion, int compatGroup, bool includeInSupportRange)
        {
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
