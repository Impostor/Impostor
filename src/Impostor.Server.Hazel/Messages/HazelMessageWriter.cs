using System;
using System.Net;
using Hazel;
using Impostor.Server.Games;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Hazel.Messages
{
    internal abstract class HazelMessageWriter : IMessageWriter
    {
        protected readonly MessageWriter Writer;

        protected HazelMessageWriter(MessageType type)
        {
            Writer = MessageWriter.Get(ToSendOption(type));
        }

        private static SendOption ToSendOption(MessageType type)
        {
            return type switch
            {
                MessageType.Unreliable => SendOption.None,
                MessageType.Reliable => SendOption.Reliable,
                _ => throw new NotSupportedException($"Message type {type} is not supported")
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Writer.Recycle();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Write(bool value)
        {
            Writer.Write(value);
        }

        public void Write(sbyte value)
        {
            Writer.Write(value);
        }

        public void Write(byte value)
        {
            Writer.Write(value);
        }

        public void Write(short value)
        {
            Writer.Write(value);
        }

        public void Write(ushort value)
        {
            Writer.Write(value);
        }

        public void Write(uint value)
        {
            Writer.Write(value);
        }

        public void Write(int value)
        {
            Writer.Write(value);
        }

        public void Write(float value)
        {
            Writer.Write(value);
        }

        public void Write(string value)
        {
            Writer.Write(value);
        }

        public void Write(IPAddress value)
        {
            Writer.Write(value.GetAddressBytes());
        }

        public void WritePacked(int value)
        {
            Writer.WritePacked(value);
        }

        public void Write(ReadOnlyMemory<byte> data)
        {
            Writer.Write(data.ToArray()); // TODO: Fix memory allocation.
        }

        public void StartMessage(byte typeFlag)
        {
            Writer.StartMessage(typeFlag);
        }

        public void Write(GameCode value)
        {
            Write(value.Value);
        }

        public void EndMessage()
        {
            Writer.EndMessage();
        }

        public void Clear(MessageType type)
        {
            Writer.Clear(ToSendOption(type));
        }
    }
}