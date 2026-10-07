using System;
using System.Collections.Generic;
using System.Threading;

namespace Impostor.Server.Localization
{
    internal sealed class LocalizationCatalogStore
    {
        private Dictionary<string, CatalogState> _catalogs = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, CatalogState> Snapshot => Volatile.Read(ref _catalogs);

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

            Volatile.Write(ref _catalogs, next);
        }

        public void Replace(Dictionary<string, CatalogState> catalogs)
        {
            Volatile.Write(ref _catalogs, catalogs);
        }
    }
}
