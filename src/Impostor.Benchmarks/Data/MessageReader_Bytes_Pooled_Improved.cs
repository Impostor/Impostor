using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Benchmarks.Data
{
    public class MessageReader_Bytes_Pooled_Improved : IDisposable
    {
        private readonly ObjectPool<MessageReader_Bytes_Pooled_Improved> _pool;

        public MessageReader_Bytes_Pooled_Improved(ObjectPool<MessageReader_Bytes_Pooled_Improved> pool)
        {
            _pool = pool;
        }

        public byte Tag { get; set; }
        public byte[] Buffer { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public int BytesRemaining => this.Length - this.Position;

        public void Update(byte[] buffer, int position = 0, int length = 0)
        {
            Tag = byte.MaxValue;
            Buffer = buffer;
            Position = position;
            Length = length;
        }

        public void Update(byte tag, byte[] buffer, int position = 0, int length = 0)
        {
            Tag = tag;
            Buffer = buffer;
            Position = position;
            Length = length;
        }

        public MessageReader_Bytes_Pooled_Improved ReadMessage()
        {
            var length = ReadUInt16();
            var tag = ReadByte();
            var pos = Position;

            Position += length;

            var result = _pool.Get();
            result.Update(tag, Buffer, pos, length);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            return Buffer[Position++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            var res = BinaryPrimitives.ReadUInt16LittleEndian(Buffer.AsSpan(Position));
            Position += sizeof(ushort);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var res = BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(Position));
            Position += sizeof(int);
            return res;
        }

        public MessageReader_Bytes_Pooled_Improved Slice(int start, int length)
        {
            var result = _pool.Get();
            result.Update(Tag, Buffer, start, length);
            return result;
        }

        public void Dispose()
        {
            _pool.Return(this);
        }
    }
}
