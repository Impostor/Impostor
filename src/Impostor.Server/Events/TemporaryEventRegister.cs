using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Impostor.Server.Events
{
    internal class TemporaryEventRegister<T>
        where T : IEvent
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly List<Func<IServiceProvider, T, ValueTask>> _callbacks = new List<Func<IServiceProvider, T, ValueTask>>();

        public async ValueTask CallAsync(IServiceProvider provider, T @event)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                foreach (var callback in _callbacks)
                {
                    await callback.Invoke(provider, @event);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public IDisposable Add(Func<IServiceProvider, T, ValueTask> callback)
        {
            semaphoreSlim.Wait();

            try
            {
                _callbacks.Add(callback);
            }
            finally
            {
                semaphoreSlim.Release();
            }

            return new UnregisterEvent(this, callback);
        }

        private void Remove(Func<IServiceProvider, T, ValueTask> callback)
        {
            semaphoreSlim.Wait();

            try
            {
                _callbacks.Remove(callback);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private class UnregisterEvent : IDisposable
        {
            private readonly TemporaryEventRegister<T> _register;
            private readonly Func<IServiceProvider, T, ValueTask> _callback;

            public UnregisterEvent(TemporaryEventRegister<T> register, Func<IServiceProvider, T, ValueTask> callback)
            {
                _register = register;
                _callback = callback;
            }

            public void Dispose()
            {
                _register.Remove(_callback);
            }
        }
    }
}