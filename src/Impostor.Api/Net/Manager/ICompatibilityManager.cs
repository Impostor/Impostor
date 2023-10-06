using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Manager
{
    /// <summary>
    /// Maintains an internal compatibility list of versions that are allowed to connect to the server, and which game
    /// versions they are allowed to play with.
    /// </summary>
    public interface ICompatibilityManager
    {
        public enum VersionCompareResult
        {
            Compatible,
            ClientTooOld,
            ServerTooOld,
            Unknown,
        }

        /// <summary>
        /// Gets the compatibility groups.
        /// </summary>
        public IEnumerable<CompatibilityGroup> CompatibilityGroups { get; }

        /// <summary>
        /// Check if a client can join the server according to the currently accepted game versions.
        /// </summary>
        /// <param name="clientVersion">The client version to check for.</param>
        /// <returns>
        /// Whether this version is supported by the server at the moment and if not, whether it is too old or too new.
        /// </returns>
        public VersionCompareResult CanConnectToServer(GameVersion clientVersion);

        /// <summary>Check if a player can join an existing game.</summary>
        /// <param name="hostVersion">The client version of the host.</param>
        /// <param name="clientVersion">The client version of the player that is joining.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><see cref="GameJoinError.None"/> if everything is OK.</item>
        ///   <item><see cref="GameJoinError.ClientOutdated"/> if the player runs a too old game version.</item>
        ///   <item><see cref="GameJoinError.ClientTooNew"/> if the player runs a too new game version.</item>
        /// </list>
        /// </returns>
        public GameJoinError CanJoinGame(GameVersion hostVersion, GameVersion clientVersion);

        /// <summary>
        /// Add a new compatibility group.
        ///
        /// WARNING: this method does not magically make changes to Impostor to properly support these versions. If
        /// Impostor cannot support the game versions you're trying to add or that the game versions you're making
        /// compatible do not crossplay correctly, weird behaviour may occur. Here be dragons.
        /// </summary>
        /// <param name="compatibilityGroup">The compatibility group to add.</param>
        public void AddCompatibilityGroup(CompatibilityGroup compatibilityGroup);

        /// <summary>
        /// Add a supported game version to the specified compatibility group.
        ///
        /// WARNING: this method does not magically make changes to Impostor to properly support these versions. If
        /// Impostor cannot support the game versions you're trying to add or that the game versions you're making
        /// compatible do not crossplay correctly, weird behaviour may occur. Here be dragons.
        /// </summary>
        /// <param name="compatibilityGroup">The compatibility group to add this version to.</param>
        /// <param name="gameVersion">The game version to add.</param>
        public void AddSupportedVersion(CompatibilityGroup compatibilityGroup, GameVersion gameVersion);

        /// <summary>
        /// Remove a version from the internal version compatibility list.
        /// </summary>
        /// Note that this will not stop players currently connected to the server from playing, it will only stop new
        /// connections.
        /// <param name="removedVersion">The version to remove from the list.</param>
        /// <returns>True iff this version was on the current compatibility list.</returns>
        public bool RemoveSupportedVersion(GameVersion removedVersion);

        public sealed class CompatibilityGroup
        {
            private readonly List<GameVersion> _gameVersions;

            public CompatibilityGroup(IEnumerable<GameVersion> gameVersions)
            {
                _gameVersions = gameVersions.ToList();
            }

            public IReadOnlyList<GameVersion> GameVersions => _gameVersions;

            public static implicit operator CompatibilityGroup(GameVersion[] gameVersions) => new(gameVersions);

            internal void Add(GameVersion gameVersion)
            {
                _gameVersions.Add(gameVersion);
            }

            internal bool Remove(GameVersion gameVersion)
            {
                return _gameVersions.Remove(gameVersion);
            }
        }
    }
}
