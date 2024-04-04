namespace Impostor.Api.Config
{
    /// <summary>
    /// Details if exceptions are made for hosts that are cheating.
    /// </summary>
    public enum CheatingHostMode
    {
        /// <summary>
        /// Hosts follow the same policies as other players.
        /// </summary>
        Never,

        /// <summary>
        /// Hosts are allowed to cheat if they request HostAuthority. If they
        /// do not request this, the same policies as for other players applies.
        /// </summary>
        /// <para>
        /// HostAuthority can be requested by hosts by adding 25 to their patch
        /// version when connecting. This flag is used by a lot of (host-only)
        /// mods and also disable server authority over MurderPlayer packets.
        /// </para>
        IfRequested,

        /// <summary>
        /// Hosts are always allowed to cheat.
        /// </summary>
        Always,
    }
}
