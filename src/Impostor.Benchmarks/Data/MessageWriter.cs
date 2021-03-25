using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;

namespace Impostor.Benchmarks.Data
{
    public class MessageWriter
    {
        public MessageType SendOption { get; private set; }

        private Stack<int> messageStarts = new Stack<int>();

        public MessageWriter(byte[] buffer)
        {
            this.Buffer = buffer;
            this.Length = this.Buffer.Length;
        }

        public MessageWriter(int bufferSize)
        {
            this.Buffer = new byte[bufferSize];
        }

        public byte[] Buffer { get; }
        public int Length { get; set; }
        public int Position { get; set; }

        public byte[] ToByteArray(bool includeHeader)
        {
            if (includeHeader)
            {
                var output = new byte[this.Length];
                System.Buffer.BlockCopy(this.Buffer, 0, output, 0, this.Length);
                return output;
            }
            else
            {
                switch (this.SendOption)
                {
                    case MessageType.Reliable:
                    {
                        var output = new byte[this.Length - 3];
                        System.Buffer.BlockCopy(this.Buffer, 3, output, 0, this.Length - 3);
                        return output;
                    }
                    case MessageType.Unreliable:
                    {
                        var output = new byte[this.Length - 1];
                        System.Buffer.BlockCopy(this.Buffer, 1, output, 0, this.Length - 1);
                        return output;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new NotImplementedException();
        }

        public bool HasBytes(int expected)
        {
            if (this.SendOption == MessageType.Unreliable)
            {
                return this.Length > 1 + expected;
            }

            return this.Length > 3 + expected;
        }

        public void Write(GameCode value)
        {
            this.Write(value.Value);
        }

        ///
        public void StartMessage(byte typeFlag)
        {
            messageStarts.Push(this.Position);
            this.Position += 2; // Skip for size
            this.Write(typeFlag);
        }

        ///
        public void EndMessage()
        {
            var lastMessageStart = messageStarts.Pop();
            var length = (ushort)(this.Position - lastMessageStart - 3); // Minus length and type byte
            this.Buffer[lastMessageStart] = (byte)length;
            this.Buffer[lastMessageStart + 1] = (byte)(length >> 8);
        }

        ///
        public void CancelMessage()
        {
            this.Position = this.messageStarts.Pop();
            this.Length = this.Position;
        }

        public void Clear(MessageType sendOption)
        {
            this.messageStarts.Clear();
            this.SendOption = sendOption;
            this.Buffer[0] = (byte)sendOption;
            switch (sendOption)
            {
                default:
                case MessageType.Unreliable:
                    this.Length = this.Position = 1;
                    break;

                case MessageType.Reliable:
                    this.Length = this.Position = 3;
                    break;
            }
        }

        #region WriteMethods

        public void Write(bool value)
        {
            this.Buffer[this.Position++] = (byte)(value ? 1 : 0);
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(sbyte value)
        {
            this.Buffer[this.Position++] = (byte)value;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(byte value)
        {
            this.Buffer[this.Position++] = value;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(short value)
        {
            this.Buffer[this.Position++] = (byte)value;
            this.Buffer[this.Position++] = (byte)(value >> 8);
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(ushort value)
        {
            this.Buffer[this.Position++] = (byte)value;
            this.Buffer[this.Position++] = (byte)(value >> 8);
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(uint value)
        {
            this.Buffer[this.Position++] = (byte)value;
            this.Buffer[this.Position++] = (byte)(value >> 8);
            this.Buffer[this.Position++] = (byte)(value >> 16);
            this.Buffer[this.Position++] = (byte)(value >> 24);
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(int value)
        {
            this.Buffer[this.Position++] = (byte)value;
            this.Buffer[this.Position++] = (byte)(value >> 8);
            this.Buffer[this.Position++] = (byte)(value >> 16);
            this.Buffer[this.Position++] = (byte)(value >> 24);
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public unsafe void Write(float value)
        {
            fixed (byte* ptr = &this.Buffer[this.Position])
            {
                var valuePtr = (byte*)&value;

                *ptr = *valuePtr;
                *(ptr + 1) = *(valuePtr + 1);
                *(ptr + 2) = *(valuePtr + 2);
                *(ptr + 3) = *(valuePtr + 3);
            }

            this.Position += 4;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(string value)
        {
            var bytes = UTF8Encoding.UTF8.GetBytes(value);
            this.WritePacked(bytes.Length);
            this.Write(bytes);
        }

        public void Write(IPAddress value)
        {
            this.Write(value.GetAddressBytes());
        }

        public void WriteBytesAndSize(byte[] bytes)
        {
            this.WritePacked((uint)bytes.Length);
            this.Write(bytes);
        }

        public void WriteBytesAndSize(byte[] bytes, int length)
        {
            this.WritePacked((uint)length);
            this.Write(bytes, length);
        }

        public void WriteBytesAndSize(byte[] bytes, int offset, int length)
        {
            this.WritePacked((uint)length);
            this.Write(bytes, offset, length);
        }

        public void Write(ReadOnlyMemory<byte> data)
        {
            Write(data.Span);
        }

        public void Write(ReadOnlySpan<byte> bytes)
        {
            bytes.CopyTo(this.Buffer.AsSpan(this.Position, bytes.Length));

            this.Position += bytes.Length;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(byte[] bytes)
        {
            Array.Copy(bytes, 0, this.Buffer, this.Position, bytes.Length);
            this.Position += bytes.Length;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(byte[] bytes, int offset, int length)
        {
            Array.Copy(bytes, offset, this.Buffer, this.Position, length);
            this.Position += length;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        public void Write(byte[] bytes, int length)
        {
            Array.Copy(bytes, 0, this.Buffer, this.Position, length);
            this.Position += length;
            if (this.Position > this.Length) this.Length = this.Position;
        }

        ///
        public void WritePacked(int value)
        {
            this.WritePacked((uint)value);
        }

        ///
        public void WritePacked(uint value)
        {
            do
            {
                var b = (byte)(value & 0xFF);
                if (value >= 0x80)
                {
                    b |= 0x80;
                }

                this.Write(b);
                value >>= 7;
            } while (value > 0);
        }

        #endregion WriteMethods

        public void Write(MessageWriter msg, bool includeHeader)
        {
            var offset = 0;
            if (!includeHeader)
            {
                switch (msg.SendOption)
                {
                    case MessageType.Unreliable:
                        offset = 1;
                        break;

                    case MessageType.Reliable:
                        offset = 3;
                        break;
                }
            }

            this.Write(msg.Buffer, offset, msg.Length - offset);
        }

        public unsafe static bool IsLittleEndian()
        {
            byte b;
            unsafe
            {
                var i = 1;
                var bp = (byte*)&i;
                b = *bp;
            }

            return b == 1;
        }
    }
}
