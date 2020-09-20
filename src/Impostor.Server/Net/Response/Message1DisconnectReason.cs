using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Response
{
    public class Message1DisconnectReason : MessageBase
    {
        private readonly DisconnectReason _reason;
        private readonly string _message;

        // Notes:
        // - Specifying no reason does something with ban minutes left.
        // - (?) You were disconnected because Among Us was suspended by another app.
        public Message1DisconnectReason(DisconnectReason reason, string message = null) : base(SendOption.Reliable, MessageFlag.DisconnectReason)
        {
            _reason = reason;
            _message = message;
        }

        protected override void WriteMessage(MessageWriter writer)
        {
            writer.Write((int) _reason);

            if (_reason == DisconnectReason.Custom)
            {
                writer.Write(_message);
            }
        }
    }
}