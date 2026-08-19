using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud
    {
        public sealed class JudgeOverrule : IInnerMeetingHud.IJudgeOverrule
        {
            public JudgeOverrule(byte judgePlayerId, byte overruledPlayerId, ushort overruleNonce)
            {
                JudgePlayerId = judgePlayerId;
                OverruledPlayerId = overruledPlayerId;
                OverruleNonce = overruleNonce;
            }

            public byte JudgePlayerId { get; set; }

            public byte OverruledPlayerId { get; set; }

            public ushort OverruleNonce { get; set; }

            public static JudgeOverrule Deserialize(IMessageReader reader)
            {
                var inner = reader.ReadMessage();
                return new JudgeOverrule(inner.Tag, inner.ReadByte(), inner.ReadUInt16());
            }
        }
    }
}
