namespace Impostor.Api.Config
{
    /// <summary>
    /// Details if exceptions are made for protocol extensions.
    /// </summary>
    public enum ProtocolExtensionsMode
    {
        /// <summary>
        /// Always forbid protocol extensions.
        /// </summary>
        Never,

        /// <summary>
        /// Protocol extensions is always needed if host request authority.
        /// </summary>
        /// <para>
        /// HostAuthority can be requested by hosts by adding 25 to their patch
        /// version when connecting. This flag is used by a lot of (host-only)
        /// mods and also disable server authority over MurderPlayer packets.
        /// </para>
        IfRequested,

        /// <summary>
        /// Protocol extensions is always allowed.
        /// </summary>
        Always,
    }
}
