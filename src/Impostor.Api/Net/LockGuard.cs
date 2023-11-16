using System;
using System.Threading;
using System.Threading.Tasks;

namespace Impostor.Api.Net
{
    /// <summary>RAII-style lock guard for use in Impostor.</summary>
    /// <remarks>
    /// To lock a semaphore, use the LockAsync method and await the result.
    /// To unlock the semaphore, dispose of the LockGuard.
    /// </remarks>
    public sealed class LockGuard : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        private bool _disposedValue;

        private LockGuard(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _semaphore.Release();
                }

                _disposedValue = true;
            }
        }

        /// <summary>Create a RAII-style lock guard for an instance of <see cref="SemaphoreSlim"/>.</summary>
        /// <param name="semaphore">The semaphore to Wait and Release on.</param>
        /// <param name="timeout">The amount of milliseconds to wait for the lock becoming available.</param>
        /// <exception cref="ImpostorException">When the timeout expires.</exception>
        /// <returns>The lock guard if the semaphore could be locked succesfully.</returns>
        public static async Task<LockGuard> LockAsync(SemaphoreSlim semaphore, int timeout = 1000)
        {
            var success = await semaphore.WaitAsync(timeout);
            if (success)
            {
                return new LockGuard(semaphore);
            }
            else
            {
                throw new ImpostorException("Could not lock semaphore");
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
