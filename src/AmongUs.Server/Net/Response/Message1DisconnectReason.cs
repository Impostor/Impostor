using AmongUs.Shared.Innersloth.Data;
using Hazel;

namespace AmongUs.Server.Net.Response
{
    public class Message1DisconnectReason : MessageBase
    {
        private readonly DisconnectReason _reason;

        // Notes:
        // - Specifying no reason does something with ban minutes left.
        // - (?) You were disconnected because Among Us was suspended by another app.
        public Message1DisconnectReason(DisconnectReason reason) : base(SendOption.Reliable, MessageFlag.DisconnectReason)
        {
            _reason = reason;
        }

        protected override void WriteMessage(MessageWriter writer)
        {
            writer.Write((int) _reason);
        }
    }
}