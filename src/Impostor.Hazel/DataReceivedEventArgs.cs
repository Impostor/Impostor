namespace Hazel
{
    public struct DataReceivedEventArgs
    {
        public readonly Connection Sender;

        /// <summary>
        ///     The bytes received from the client.
        /// </summary>
        public readonly MessageReader Message;

        /// <summary>
        ///     The <see cref="SendOption"/> the data was sent with.
        /// </summary>
        public readonly SendOption SendOption;
        
        public DataReceivedEventArgs(Connection sender, MessageReader msg, SendOption sendOption)
        {
            this.Sender = sender;
            this.Message = msg;
            this.SendOption = sendOption;
        }
    }
}
