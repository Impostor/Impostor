using System;
using System.Threading;
using Impostor.Api.Localization;

namespace Impostor.Server.Localization
{
    internal sealed class LocalizationRegistration : ILocalizationRegistration
    {
        private readonly LocalizationService _service;
        private readonly string _owner;
        private readonly Guid _guid;
        private int _disposed;

        public LocalizationRegistration(LocalizationService service, string owner, Guid guid)
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
