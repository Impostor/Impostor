using System.Reflection;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Localization
{
    public sealed class LocalizationCatalog
    {
        /// <summary>Initializes a new instance of the <see cref="LocalizationCatalog"/> class with resources named prefix.language-code.json.</summary>
        /// <param name="owner">The unique plugin ID.</param>
        /// <param name="assembly">The assembly containing the JSON resources.</param>
        /// <param name="defaultLanguage">The language containing every message key.</param>
        public LocalizationCatalog(string owner, Assembly assembly, Language defaultLanguage = Language.English)
        {
            Owner = owner;
            Assembly = assembly;
            DefaultLanguage = defaultLanguage;
        }

        public string Owner { get; }

        public Assembly Assembly { get; }

        public Language DefaultLanguage { get; }
    }
}
