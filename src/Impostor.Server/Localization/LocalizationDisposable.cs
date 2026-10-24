using System;
using System.Threading;

namespace Impostor.Server.Localization
{
    internal sealed class LocalizationDisposable : IDisposable
    {
        private readonly LocalizationService _service;
        private readonly string _owner;
        private readonly Guid _guid;
        private int _disposed;

        public LocalizationDisposable(LocalizationService service, string owner, Guid guid)
        {
            _service = service;
            _owner = owner;
            _guid = guid;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _service.Unregister(_owner, _guid);
            }
        }
    }
}
