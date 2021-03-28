using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Impostor.Server.Events.Register
{
    internal class TemporaryEventRegister
    {
        private readonly ConcurrentDictionary<int, IRegisteredEventListener> _callbacks;
        private int _idLast;

        public TemporaryEventRegister()
        {
            _callbacks = new ConcurrentDictionary<int, IRegisteredEventListener>();
        }

        public IEnumerable<IRegisteredEventListener> GetEventListeners()
        {
            return _callbacks.Select(i => i.Value);
        }

        public IDisposable Add(IRegisteredEventListener callback)
        {
            var id = Interlocked.Increment(ref _idLast);

            if (!_callbacks.TryAdd(id, callback))
            {
                Debug.Fail("Failed to register the event listener");
            }

            return new UnregisterEvent(this, id);
        }

        private void Remove(int id)
        {
            _callbacks.TryRemove(id, out _);
        }

        private class UnregisterEvent : IDisposable
        {
            private readonly TemporaryEventRegister _register;
            private readonly int _id;

            public UnregisterEvent(TemporaryEventRegister register, int id)
            {
                _register = register;
                _id = id;
            }

            public void Dispose()
            {
                _register.Remove(_id);
            }
        }
    }
}
