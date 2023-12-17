using System;
using System.Threading;
using System.Threading.Tasks;

namespace Impostor.Api.Utils;

/// <summary>
/// Represents an asynchronous lock mechanism.
/// </summary>
public sealed class AsyncLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Asynchronously acquires a lock.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{Releaser}"/> that represents the asynchronous lock operation.
    /// The result of the task is a <see cref="Releaser"/> that should be disposed to release the lock.
    /// </returns>
    public ValueTask<Releaser> LockAsync()
    {
        return LockAsync(CancellationToken.None);
    }

    /// <inheritdoc cref="LockAsync()"/>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the lock.</param>
    /// <exception cref="T:System.OperationCanceledException"><paramref name="cancellationToken" /> was canceled.</exception>
    public ValueTask<Releaser> LockAsync(CancellationToken cancellationToken)
    {
        var wait = _semaphore.WaitAsync(cancellationToken);

        if (wait.IsCompleted)
        {
            return ValueTask.FromResult(new Releaser(this));
        }

        return new ValueTask<Releaser>(wait.ContinueWith(
            static (_, state) => new Releaser((AsyncLock)state!),
            this,
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        ));
    }

    /// <summary>
    /// Represents a disposable object that, when disposed, releases the associated lock.
    /// </summary>
    public struct Releaser : IDisposable
    {
        private AsyncLock? _lock;

        internal Releaser(AsyncLock @lock)
        {
            _lock = @lock;
        }

        /// <summary>
        /// Releases the lock held by this <see cref="Releaser"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (_lock != null)
            {
                _lock._semaphore.Release();
                _lock = null;
            }
        }
    }
}
