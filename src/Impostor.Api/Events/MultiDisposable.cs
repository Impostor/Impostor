using System;
using System.Collections.Generic;
using System.Linq;

namespace Impostor.Api.Events;

/// <summary>
///     Disposes multiple <see cref="IDisposable" />.
/// </summary>
public class MultiDisposable(IEnumerable<IDisposable> disposables) : IDisposable
{
    public MultiDisposable(params IDisposable[] disposables) : this(disposables.AsEnumerable())
    {
    }

    public void Dispose()
    {
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }
    }
}
