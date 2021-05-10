using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Impostor.Api;
using Impostor.Api.Net.Custom;

namespace Impostor.Server.Net.Custom
{
    public class CustomMessageManager<T> : ICustomMessageManager<T>
        where T : ICustomMessage
    {
        private readonly ConcurrentDictionary<byte, T> _messages = new ConcurrentDictionary<byte, T>();

        public bool TryGet(byte id, [MaybeNullWhen(false)] out T message)
        {
            return _messages.TryGetValue(id, out message);
        }

        public IDisposable Register(T message)
        {
            if (!_messages.TryAdd(message.Id, message))
            {
                throw new ImpostorException($"Message with id: {message.Id} was already registered");
            }

            return new UnregisterDisposable(this, message);
        }

        private class UnregisterDisposable : IDisposable
        {
            private readonly CustomMessageManager<T> _manager;
            private readonly T _message;

            private bool _disposed;

            public UnregisterDisposable(CustomMessageManager<T> manager, T message)
            {
                _manager = manager;
                _message = message;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("Tried to dispose already disposed object");
                }

                _manager._messages.TryRemove(_message.Id, out _);
                _disposed = true;
            }
        }
    }
}
