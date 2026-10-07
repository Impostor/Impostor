using System.Collections.Generic;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Localization
{
    internal sealed record LoadedCatalog(
        string Owner,
        Language DefaultLanguage,
        IReadOnlyDictionary<Language, IReadOnlyDictionary<string, string>> Translations);
}
