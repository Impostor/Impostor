using System;
using System.Collections.Generic;
using System.Linq;

namespace Impostor.Api.Events
{
    /// <summary>
    ///     Disposes multiple <see cref="IDisposable" />.
    /// </summary>
    public class MultiDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> _disposables;

        public MultiDisposable(IEnumerable<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public MultiDisposable(params IDisposable[] disposables) : this(disposables.AsEnumerable())
        {
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
