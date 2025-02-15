using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Impostor.Server.Events.Register;

internal class TemporaryEventRegister
{
    private readonly ConcurrentDictionary<int, IRegisteredEventListener> _callbacks = new();
    private int _idLast;

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

    private class UnregisterEvent(TemporaryEventRegister register, int id) : IDisposable
    {
        public void Dispose()
        {
            register.Remove(id);
        }
    }
}
