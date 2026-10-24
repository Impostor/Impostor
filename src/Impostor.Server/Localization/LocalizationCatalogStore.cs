using System;
using System.Collections.Generic;

namespace Impostor.Server.Localization
{
    internal sealed class LocalizationCatalogStore
    {
        private Dictionary<string, CatalogState> _catalogs = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, CatalogState> Snapshot => _catalogs;

        public void Publish(string owner, CatalogState? state)
        {
            var next = new Dictionary<string, CatalogState>(_catalogs, StringComparer.OrdinalIgnoreCase);
            if (state == null)
            {
                next.Remove(owner);
            }
            else
            {
                next[owner] = state;
            }

            _catalogs = next;
        }

        public void Replace(Dictionary<string, CatalogState> catalogs)
        {
            _catalogs = catalogs;
        }
    }
}
