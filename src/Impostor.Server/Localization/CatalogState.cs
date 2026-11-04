using System;
using System.Collections.Generic;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Localization
{
    internal sealed record CatalogState(
        Guid Token,
        LoadedCatalog Defaults,
        IReadOnlyDictionary<Language, IReadOnlyDictionary<string, string>> Overrides);
}
