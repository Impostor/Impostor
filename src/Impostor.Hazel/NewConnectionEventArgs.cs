namespace Hazel
{
    public struct NewConnectionEventArgs
    {
        /// <summary>
        /// The data received from the client in the handshake.
        /// This data is yours. Remember to recycle it.
        /// </summary>
        public readonly MessageReader HandshakeData;

        /// <summary>
        /// The <see cref="Connection"/> to the new client.
        /// </summary>
        public readonly Connection Connection;

        public NewConnectionEventArgs(MessageReader handshakeData, Connection connection)
        {
            this.HandshakeData = handshakeData;
            this.Connection = connection;
        }
    }
}
