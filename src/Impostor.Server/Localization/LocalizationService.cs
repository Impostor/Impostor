using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Localization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Localization
{
    public sealed class LocalizationService : ILocalizationService, ILocalizationRegistry, IHostedService, IDisposable
    {
        private readonly object _gate = new();
        private readonly ILogger<LocalizationService> _logger;
        private readonly LocalizationCatalogStore _store = new();
        private readonly LocalizationFileStore _files;
        private readonly HashSet<string> _initializedOwners = new(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        public LocalizationService(ILogger<LocalizationService> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _files = new LocalizationFileStore(environment.ContentRootPath, logger);
            if (LocalizationResourceLoader.TryLoad(BuiltInMessages.Load(), logger, out var defaults))
            {
                Initialize(defaults!);
            }
            else
            {
                _logger.LogError("Built-in localization resources could not be loaded");
            }
        }

        public string Get(LocalizedMessageKey message, Language language, params object?[] arguments)
        {
            CatalogState? state = null;
            if (!string.IsNullOrWhiteSpace(message.Owner))
            {
                _store.Snapshot.TryGetValue(message.Owner, out state);
            }

            return LocalizationFormatter.Get(state, message, language, arguments, _logger);
        }

        public bool TryRegister(LocalizationCatalog catalog, out IDisposable? disposable)
        {
            disposable = null;
            lock (_gate)
            {
                if (_disposed)
                {
                    _logger.LogWarning("Cannot register localization resources: service is disposed");
                    return false;
                }

                if (catalog == null || string.Equals(catalog.Owner, "impostor", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Cannot register localization resources: catalog is missing or owner is reserved");
                    return false;
                }

                if (catalog.Owner != null && _initializedOwners.Contains(catalog.Owner))
                {
                    _logger.LogWarning("Localization owner {Owner} was already initialized; restart the server to reload resources", catalog.Owner);
                    return false;
                }

                if (!LocalizationResourceLoader.TryLoad(catalog, _logger, out var defaults))
                {
                    return false;
                }

                var token = Initialize(defaults!);
                disposable = new LocalizationDisposable(this, defaults!.Owner, token);
                return true;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _disposed = true;
            }
        }

        internal void Unregister(string owner, Guid token)
        {
            lock (_gate)
            {
                if (_store.Snapshot.TryGetValue(owner, out var state) && state.Token == token)
                {
                    _store.Publish(owner, null);
                }
            }
        }

        private Guid Initialize(LoadedCatalog defaults)
        {
            var token = Guid.NewGuid();
            _files.EnsureFiles(defaults);
            var state = new CatalogState(token, defaults, new Dictionary<Language, IReadOnlyDictionary<string, string>>());
            _store.Publish(defaults.Owner, _files.LoadOverrides(state));
            _initializedOwners.Add(defaults.Owner);
            return token;
        }
    }
}
