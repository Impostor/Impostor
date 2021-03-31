using System;
using System.Buffers.Binary;

namespace Impostor.Benchmarks.Data.Span
{
    public ref struct MessageReader_Span
    {
        private readonly MessageReaderOwner _owner;
        private readonly byte _tag;
        private readonly Span<byte> _data;

        public MessageReader_Span(MessageReaderOwner owner, byte tag, Span<byte> data)
        {
            _owner = owner;
            _tag = tag;
            _data = data;
        }

        public MessageReader_Span ReadMessage()
        {
            var length = ReadUInt16();
            var tag = ReadByte();
            var pos = _owner.Position;

            _owner.Position += length;

            return new MessageReader_Span(_owner, tag, _data.Slice(3, length));
        }

        public byte ReadByte()
        {
            return _data[_owner.Position++];
        }

        public ushort ReadUInt16()
        {
            var output = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_owner.Position));
            _owner.Position += sizeof(ushort);
            return output;
        }

        public int ReadInt32()
        {
            var output = BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(_owner.Position));
            _owner.Position += sizeof(int);
            return output;
        }
    }
}
