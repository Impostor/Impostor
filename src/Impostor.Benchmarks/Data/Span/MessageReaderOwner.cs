using System;

namespace Impostor.Benchmarks.Data.Span
{
    public class MessageReaderOwner
    {
        private readonly Memory<byte> _data;

        public MessageReaderOwner(Memory<byte> data)
        {
            _data = data;
        }

        public int Position { get; internal set; }
        public int Length => _data.Length;

        public MessageReader_Span CreateReader()
        {
            return new MessageReader_Span(this, byte.MaxValue, _data.Span.Slice(Position));
        }
    }
}
