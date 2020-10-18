using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Hazel.Messages
{
    public class BufferMessageReader : IMessageReader
    {
        public byte Tag { get; }
        public ReadOnlyMemory<byte> Buffer { get; }
        public int Position { get; set; }
        public int Length => Buffer.Length;

        public BufferMessageReader(byte tag, ReadOnlyMemory<byte> buffer)
        {
            Tag = tag;
            Buffer = buffer;
        }

        public IMessageReader ReadMessage()
        {
            var length = ReadUInt16();
            var tag = ReadByte();
            var pos = Position;

            Position += length;

            return new BufferMessageReader(tag, Buffer.Slice(pos, length));
        }

        public bool ReadBoolean()
        {
            byte val = FastByte();
            return val != 0;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)FastByte();
        }

        public byte ReadByte()
        {
            return FastByte();
        }

        public ushort ReadUInt16()
        {
            var output = BinaryPrimitives.ReadUInt16LittleEndian(Buffer.Span.Slice(Position));
            Position += sizeof(ushort);
            return output;
        }

        public short ReadInt16()
        {
            var output = BinaryPrimitives.ReadInt16LittleEndian(Buffer.Span.Slice(Position));
            Position += sizeof(short);
            return output;
        }

        public uint ReadUInt32()
        {
            var output = BinaryPrimitives.ReadUInt32LittleEndian(Buffer.Span.Slice(Position));
            Position += sizeof(uint);
            return output;
        }

        public int ReadInt32()
        {
            var output = BinaryPrimitives.ReadInt32LittleEndian(Buffer.Span.Slice(Position));
            Position += sizeof(int);
            return output;
        }

        public float ReadSingle()
        {
            var output = BinaryPrimitives.ReadSingleLittleEndian(Buffer.Span.Slice(Position));
            Position += sizeof(float);
            return output;
        }

        public string ReadString()
        {
            var len = ReadPackedInt32();
            var output = Encoding.UTF8.GetString(Buffer.Span.Slice(Position, len));
            Position += len;
            return output;
        }

        public ReadOnlyMemory<byte> ReadBytesAndSize()
        {
            var len = ReadPackedInt32();
            return ReadBytes(len);
        }

        public ReadOnlyMemory<byte> ReadBytes(int length)
        {
            var output = Buffer.Slice(Position, length);
            Position += length;
            return output;
        }

        public int ReadPackedInt32()
        {
            return (int)ReadPackedUInt32();
        }

        public uint ReadPackedUInt32()
        {
            bool readMore = true;
            int shift = 0;
            uint output = 0;

            while (readMore)
            {
                byte b = ReadByte();
                if (b >= 0x80)
                {
                    readMore = true;
                    b ^= 0x80;
                }
                else
                {
                    readMore = false;
                }

                output |= (uint)(b << shift);
                shift += 7;
            }

            return output;
        }

        public void CopyTo(IMessageWriter writer)
        {
            writer.Write((ushort) Length);
            writer.Write(Tag);
            writer.Write(Buffer);
        }

        public IMessageReader Slice(int start)
        {
            return new BufferMessageReader(Tag, Buffer.Slice(start));
        }

        public IMessageReader Slice(int start, int length)
        {
            return new BufferMessageReader(Tag, Buffer.Slice(start, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte FastByte()
        {
            return Buffer.Span[Position++];
        }
    }
}