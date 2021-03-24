namespace Impostor.Hazel
{
    /// <summary>
    ///     Represents the IP version that a connection or listener will use.
    /// </summary>
    /// <remarks>
    ///     If you wand a client to connect or be able to connect using IPv6 then you should use <see cref="IPv4AndIPv6"/>, 
    ///     this sets the underlying sockets to use IPv6 but still allow IPv4 sockets to connect for backwards compatability 
    ///     and hence it is the default IPMode in most cases.
    /// </remarks>
    public enum IPMode
    {
        /// <summary>
        ///     Instruction to use IPv4 only, IPv6 connections will not be able to connect.
        /// </summary>
        IPv4,

        /// <summary>
        ///     Instruction to use IPv6 only, IPv4 connections will not be able to connect. IPv4 addresses can be connected 
        ///     by converting to IPv6 addresses.
        /// </summary>
        IPv6
    }
}
