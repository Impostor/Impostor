using System;
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
            // TODO: Refactor to System.Buffers.Binary.BinaryPrimitives
            
            ushort output =
                (ushort)(FastByte()
                         | FastByte() << 8);
            return output;
        }

        public short ReadInt16()
        {
            // TODO: Refactor to System.Buffers.Binary.BinaryPrimitives
            
            short output =
                (short)(FastByte() | FastByte() << 8);
            return output;
        }

        public uint ReadUInt32()
        {
            // TODO: Refactor to System.Buffers.Binary.BinaryPrimitives
            
            uint output = FastByte()
                          | (uint)FastByte() << 8
                          | (uint)FastByte() << 16
                          | (uint)FastByte() << 24;

            return output;
        }

        public int ReadInt32()
        {
            // TODO: Refactor to System.Buffers.Binary.BinaryPrimitives
            
            int output = FastByte()
                         | FastByte() << 8
                         | FastByte() << 16
                         | FastByte() << 24;

            return output;
        }

        public unsafe float ReadSingle()
        {
            // TODO: Refactor to System.Buffers.Binary.BinaryPrimitives
            
            float output = 0;
            fixed (byte* bufPtr = &Buffer.Span[Position])
            {
                byte* outPtr = (byte*)&output;

                *outPtr = *bufPtr;
                *(outPtr + 1) = *(bufPtr + 1);
                *(outPtr + 2) = *(bufPtr + 2);
                *(outPtr + 3) = *(bufPtr + 3);
            }

            Position += 4;
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
            writer.Write((byte) Tag);
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