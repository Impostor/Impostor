using System;
using Hazel;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Hazel.Messages
{
    internal class HazelMessage : IMessage, IDisposable
    {
        private bool _isDisposed;
        private readonly MessageReader _reader;

        public HazelMessage(MessageReader reader, MessageType type)
        {
            _reader = reader;
            Type = type;
        }

        public MessageType Type { get; }

        public IMessageReader CreateReader()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(_reader));
            }

            return new BufferMessageReader(_reader.Tag, _reader.Buffer);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HazelMessage()
        {
            Dispose(false);
        }
    }
}