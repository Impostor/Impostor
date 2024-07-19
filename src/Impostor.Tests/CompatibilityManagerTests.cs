using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Manager;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Impostor.Tests;

using CompatibilityGroup = ICompatibilityManager.CompatibilityGroup;
using VersionCompareResult = ICompatibilityManager.VersionCompareResult;

public sealed class CompatibilityManagerTests
{
    private static readonly CompatibilityGroup[] DefaultSupportedVersions =
    {
        new[]
        {
            new GameVersion(1, 0, 0),
        },

        new[]
        {
            new GameVersion(2, 0, 0),
            new GameVersion(2, 1, 0),
        },
    };

    private readonly CompatibilityManager _compatibilityManager = new(NullLogger<CompatibilityManager>.Instance, DefaultSupportedVersions);

    public static IEnumerable<object[]> CanConnectToServerData =>
        new List<object[]>
        {
            new object[] { VersionCompareResult.ClientTooOld, new GameVersion(0, 0, 0) },
            new object[] { VersionCompareResult.ServerTooOld, new GameVersion(100, 0, 0) },
            new object[] { VersionCompareResult.Unknown, new GameVersion(2, 0, 1) },
        };

    [Theory]
    [MemberData(nameof(CanConnectToServerData))]
    public void CanConnectToServer(VersionCompareResult versionCompareResult, GameVersion gameVersion)
    {
        Assert.Equal(versionCompareResult, _compatibilityManager.CanConnectToServer(gameVersion));
    }

    public static IEnumerable<object[]> CanJoinGameData =>
        new List<object[]>
        {
            // Same version -> can join
            new object[] { GameJoinError.None, new GameVersion(1, 0, 0), new GameVersion(1, 0, 0) },

            // Compatible versions -> can join
            new object[] { GameJoinError.None, new GameVersion(2, 0, 0), new GameVersion(2, 1, 0) },

            // Different incompatible versions -> show correct error
            new object[] { GameJoinError.ClientOutdated, new GameVersion(2, 0, 0), new GameVersion(1, 0, 0) },
            new object[] { GameJoinError.ClientTooNew, new GameVersion(1, 0, 0), new GameVersion(2, 0, 0) },

            // Versions unknown to Impostor -> also show correct errors
            new object[] { GameJoinError.ClientTooNew, new GameVersion(0, 0, 1), new GameVersion(1, 0, 0) },
            new object[] { GameJoinError.ClientOutdated, new GameVersion(1, 0, 0), new GameVersion(0, 0, 1) },
            new object[] { GameJoinError.ClientTooNew, new GameVersion(1, 0, 0), new GameVersion(100, 0, 0) },
            new object[] { GameJoinError.ClientOutdated, new GameVersion(100, 0, 0), new GameVersion(1, 0, 0) },
            new object[] { GameJoinError.ClientTooNew, new GameVersion(0, 0, 1), new GameVersion(100, 0, 0) },

        };

    [Theory]
    [MemberData(nameof(CanJoinGameData))]
    public void CanJoinGame(GameJoinError gameJoinError, GameVersion hostVersion, GameVersion clientVersion)
    {
        Assert.Equal(gameJoinError, _compatibilityManager.CanJoinGame(hostVersion, clientVersion));
    }

    public static IEnumerable<object[]> CanConnectAndJoinData =>
        new List<object[]>
        {
            new object[] { new GameVersion(1, 0, 0), new GameVersion(1, 0, 0) },
            new object[] { new GameVersion(2, 0, 0), new GameVersion(2, 0, 0) },
            new object[] { new GameVersion(2, 1, 0), new GameVersion(2, 0, 0) },
            new object[] { new GameVersion(2, 0, 0), new GameVersion(2, 1, 0) },
            new object[] { new GameVersion(2, 0, 0), new GameVersion(2, 0, 0, 25) }, // server authority flag
        };

    [Theory]
    [MemberData(nameof(CanConnectAndJoinData))]
    public void CanConnectAndJoin(GameVersion hostVersion, GameVersion clientVersion)
    {
        Assert.Equal(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(hostVersion));
        Assert.Equal(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(clientVersion));

        Assert.Equal(GameJoinError.None, _compatibilityManager.CanJoinGame(hostVersion, clientVersion));
    }

    [Fact]
    public void PublicApi()
    {
        ICompatibilityManager compatibilityManager = _compatibilityManager;

        {
            Assert.NotEqual(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(new GameVersion(2, 2, 0)));
            Assert.NotEqual(GameJoinError.None, _compatibilityManager.CanJoinGame(new GameVersion(2, 2, 0), new GameVersion(2, 1, 0)));

            var groupV2 = compatibilityManager.CompatibilityGroups.Single(g => g.GameVersions.Contains(new GameVersion(2, 0, 0)));
            compatibilityManager.AddSupportedVersion(groupV2, new GameVersion(2, 2, 0));

            Assert.Equal(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(new GameVersion(2, 2, 0)));
            Assert.Equal(GameJoinError.None, _compatibilityManager.CanJoinGame(new GameVersion(2, 2, 0), new GameVersion(2, 1, 0)));
        }

        {
            Assert.NotEqual(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(new GameVersion(3, 0, 0)));

            compatibilityManager.AddCompatibilityGroup(new[] { new GameVersion(3, 0, 0), });

            Assert.Equal(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(new GameVersion(3, 0, 0)));
        }

        {
            Assert.Equal(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(new GameVersion(1, 0, 0)));

            compatibilityManager.RemoveSupportedVersion(new GameVersion(1, 0, 0));

            Assert.NotEqual(VersionCompareResult.Compatible, _compatibilityManager.CanConnectToServer(new GameVersion(1, 0, 0)));
        }
    }
}
