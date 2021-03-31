using System;
using System.Collections.Generic;

namespace Impostor.Server.Events
{
    /// <summary>
    ///     Disposes multiple <see cref="IDisposable" />.
    /// </summary>
    internal class MultiDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> _disposables;

        public MultiDisposable(IEnumerable<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }
    }
}
