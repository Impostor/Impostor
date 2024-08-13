using System;
using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Manager;

using CompatibilityGroup = ICompatibilityManager.CompatibilityGroup;
using VersionCompareResult = ICompatibilityManager.VersionCompareResult;

internal class CompatibilityManager : ICompatibilityManager
{
    private static readonly CompatibilityGroup[] DefaultSupportedVersions =
    {
        new[]
        {
            new GameVersion(2024, 3, 1), // 2024.6.18
            new GameVersion(2024, 4, 1), // 2024.8.13
        },
    };

    private readonly List<CompatibilityGroup> _compatibilityGroups = new();
    private readonly Dictionary<GameVersion, CompatibilityGroup> _supportMap = new();
    private readonly ILogger<CompatibilityManager> _logger;
    private GameVersion _lowestVersionSupported = new(int.MaxValue);
    private GameVersion _highestVersionSupported = new(0);

    public CompatibilityManager(ILogger<CompatibilityManager> logger) : this(logger, DefaultSupportedVersions)
    {
    }

    internal CompatibilityManager(ILogger<CompatibilityManager> logger, IEnumerable<CompatibilityGroup> defaultSupportedVersions)
    {
        _logger = logger;

        foreach (var compatibilityGroup in defaultSupportedVersions)
        {
            AddCompatibilityGroup(compatibilityGroup);
        }
    }

    public IEnumerable<CompatibilityGroup> CompatibilityGroups => _compatibilityGroups;

    private CompatibilityGroup? TryGetCompatibilityGroup(GameVersion clientVersion)
    {
        // Innersloth servers allow disabling server authority by incrementing the version revision by 25.
        // We should allow crossplay between client versions with this flag set and those without.
        clientVersion = clientVersion.Normalize();

        if (_supportMap.TryGetValue(clientVersion, out var compatibilityGroup))
        {
            return compatibilityGroup;
        }

        return null;
    }

    private CompatibilityGroup GetCompatibilityGroupOrDefault(GameVersion clientVersion)
    {
        // If the compatibility group is not defined, we assume it is not compatible with anything else than itself
        return TryGetCompatibilityGroup(clientVersion) ?? new CompatibilityGroup(new[] { clientVersion.Normalize() });
    }

    public VersionCompareResult CanConnectToServer(GameVersion clientVersion)
    {
        if (this.TryGetCompatibilityGroup(clientVersion) != null)
        {
            return VersionCompareResult.Compatible;
        }

        if (clientVersion < _lowestVersionSupported)
        {
            return VersionCompareResult.ClientTooOld;
        }

        if (clientVersion > _highestVersionSupported)
        {
            return VersionCompareResult.ServerTooOld;
        }

        return VersionCompareResult.Unknown;
    }

    public GameJoinError CanJoinGame(GameVersion hostVersion, GameVersion clientVersion)
    {
        if (hostVersion == clientVersion)
        {
            // Optimize a common case: a player on version X should always be able to join version X
            return GameJoinError.None;
        }

        var hostCompatGroup = GetCompatibilityGroupOrDefault(hostVersion);
        var playerCompatGroup = GetCompatibilityGroupOrDefault(clientVersion);

        if (hostCompatGroup != playerCompatGroup)
        {
            return clientVersion < hostVersion
                ? GameJoinError.ClientOutdated
                : GameJoinError.ClientTooNew;
        }

        return GameJoinError.None;
    }

    void ICompatibilityManager.AddCompatibilityGroup(CompatibilityGroup compatibilityGroup)
    {
        _logger.LogWarning($"{nameof(AddCompatibilityGroup)} was called by a plugin, this can create unexpected issues. Please proceed carefully");

        AddCompatibilityGroup(compatibilityGroup);
    }

    void ICompatibilityManager.AddSupportedVersion(CompatibilityGroup compatibilityGroup, GameVersion gameVersion)
    {
        _logger.LogWarning($"{nameof(AddSupportedVersion)} was called by a plugin, this can create unexpected issues. Please proceed carefully");

        if (compatibilityGroup.GameVersions.Contains(gameVersion))
        {
            return;
        }

        AddSupportedVersion(compatibilityGroup, gameVersion, true);
    }

    private void AddCompatibilityGroup(CompatibilityGroup compatibilityGroup)
    {
        foreach (var gameVersion in compatibilityGroup.GameVersions)
        {
            if (_supportMap.ContainsKey(gameVersion))
            {
                throw new InvalidOperationException($"Can't add this compatibility group because one if its versions ({gameVersion}) is already added");
            }
        }

        _compatibilityGroups.Add(compatibilityGroup);

        foreach (var gameVersion in compatibilityGroup.GameVersions)
        {
            AddSupportedVersion(compatibilityGroup, gameVersion, false);
        }
    }

    private void AddSupportedVersion(CompatibilityGroup compatibilityGroup, GameVersion gameVersion, bool addToGroup)
    {
        if (!_compatibilityGroups.Contains(compatibilityGroup))
        {
            throw new InvalidOperationException("You have to add the compatibility group first");
        }

        if (_supportMap.ContainsKey(gameVersion))
        {
            throw new InvalidOperationException("Can't add this game version because it's already in another compatibility group");
        }

        if (addToGroup)
        {
            compatibilityGroup.Add(gameVersion);
        }

        _supportMap.Add(gameVersion, compatibilityGroup);

        if (gameVersion < _lowestVersionSupported)
        {
            _lowestVersionSupported = gameVersion;
        }

        if (gameVersion > _highestVersionSupported)
        {
            _highestVersionSupported = gameVersion;
        }
    }

    public bool RemoveSupportedVersion(GameVersion removedVersion)
    {
        if (_supportMap.Remove(removedVersion, out var compatibilityGroup))
        {
            if (!compatibilityGroup.Remove(removedVersion))
            {
                throw new InvalidOperationException("Removed the version from the support map but it was missing from it's compatibility group");
            }

            if (!compatibilityGroup.GameVersions.Any())
            {
                _compatibilityGroups.Remove(compatibilityGroup);
            }

            return true;
        }

        return false;
    }
}
