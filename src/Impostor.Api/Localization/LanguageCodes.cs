using System.Collections.Generic;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Localization
{
    public static class LanguageCodes
    {
        public static IReadOnlyDictionary<Language, string> All { get; } = new Dictionary<Language, string>
        {
            [Language.English] = "en",
            [Language.Latam] = "es-419",
            [Language.Brazilian] = "pt-BR",
            [Language.Portuguese] = "pt-PT",
            [Language.Korean] = "ko",
            [Language.Russian] = "ru",
            [Language.Dutch] = "nl",
            [Language.Filipino] = "fil",
            [Language.French] = "fr",
            [Language.German] = "de",
            [Language.Italian] = "it",
            [Language.Japanese] = "ja",
            [Language.Spanish] = "es-ES",
            [Language.SChinese] = "zh-CN",
            [Language.TChinese] = "zh-TW",
            [Language.Irish] = "ga",
        };
    }
}
