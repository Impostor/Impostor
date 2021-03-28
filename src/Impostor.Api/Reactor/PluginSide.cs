namespace Impostor.Api.Reactor
{
    /// <summary>
    ///     Plugin side used in modded handshake.
    /// </summary>
    public enum PluginSide : byte
    {
        /// <summary>
        ///     Required by both sides, reject connection if missing on the other side
        /// </summary>
        Both,

        /// <summary>
        ///     Required only by client
        /// </summary>
        ClientOnly,

        /// <summary>
        ///     Required only by server
        /// </summary>
        ServerOnly,
    }
}
