using System;

namespace Impostor.Api.Localization
{
    public interface ILocalizationRegistry
    {
        /// <summary>
        /// Registers an independently owned catalog.
        /// Owners must be unique and consist of ASCII letters, digits, dots, underscores or hyphens. "impostor" is reserved.
        /// Dispose the returned handle when the plugin is disabled.
        /// Register during server startup; each owner is initialized only once per session.
        /// </summary>
        /// <param name="catalog">Default translations owned by the plugin.</param>
        /// <param name="disposable">The disposable registration handle, or null on failure.</param>
        /// <returns>True on success; failures are logged without throwing validation exceptions.</returns>
        bool TryRegister(LocalizationCatalog catalog, out IDisposable? disposable);
    }
}
