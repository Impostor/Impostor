namespace Impostor.Api.Net.Manager
{
    /**
     * <summary>
     * Maintain an internal compatibility list of versions that are allowed to connect to the server, and which game
     * versions they are allowed to play with.
     * </summary>
     */
    public interface ICompatibilityManager
    {
        public enum VersionCompareResult
        {
            Compatible,
            ClientTooOld,
            ServerTooOld,
            Unknown,
        }

        /**
         * <summary>
         * Get the compatibility group of a certain client version. If two versions are in the same compatibility group,
         * they can play a game together without issues.
         * </summary>
         * <param name="clientVersion">The client version to check for.</param>
         * <param name="compatGroup">If the result is Compatible, the compat group.</param>
         * <returns>
         * Whether this version is supported by the server at the moment and if not, whether it is too old or too new.
         * </returns>
         */
        public VersionCompareResult TryGetCompatibilityGroup(int clientVersion, out int? compatGroup);

        /**
         * <summary>
         * Add a supported game version to the internal version compatibility list. If this version is already on the
         * compatibility list, modify its configuration.
         *
         * WARNING: this method does not magically make changes to Impostor to properly support these versions. If
         * Impostor cannot support the game versions you're trying to add or that the game versions you're making
         * compatible do not crossplay correctly, weird behaviour may occur. Here be dragons.
         * </summary>
         * <param name="gameVersion">The game version to add details to.</param>
         * <param name="compatGroup">The compatibility group to add this version to. Customary is to use the broadcast
         * version of the lowest game version it is compatible with.</param>
         * <param name="includeInSupportRange">
         * Whether to add this version to the list of minimum/maximum game versions supported.
         * </param>
         */
        public void AddSupportedVersion(int gameVersion, int compatGroup, bool includeInSupportRange = true);

        /**
         * <summary>
         * Remove a version from the internal version compatibility list.
         * </summary>
         * Note that this will not stop players currently connected to the server from playing, it will only stop new
         * connections.
         * <param name="removedVersion">The version to remove from the list.</param>
         * <returns>True iff this version was on the current compatibility list.</returns>
         */
        public bool RemoveSupportedVersion(int removedVersion);
    }
}
