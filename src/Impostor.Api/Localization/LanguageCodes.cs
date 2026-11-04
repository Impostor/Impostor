using System.Collections.Generic;
using System.Globalization;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Localization
{
    public static class LanguageCodes
    {
        public static IReadOnlyDictionary<Language, CultureInfo> All { get; } = new Dictionary<Language, CultureInfo>
        {
            [Language.English] = CultureInfo.GetCultureInfo("en"),
            [Language.Latam] = CultureInfo.GetCultureInfo("es-419"),
            [Language.Brazilian] = CultureInfo.GetCultureInfo("pt-BR"),
            [Language.Portuguese] = CultureInfo.GetCultureInfo("pt-PT"),
            [Language.Korean] = CultureInfo.GetCultureInfo("ko"),
            [Language.Russian] = CultureInfo.GetCultureInfo("ru"),
            [Language.Dutch] = CultureInfo.GetCultureInfo("nl"),
            [Language.Filipino] = CultureInfo.GetCultureInfo("fil"),
            [Language.French] = CultureInfo.GetCultureInfo("fr"),
            [Language.German] = CultureInfo.GetCultureInfo("de"),
            [Language.Italian] = CultureInfo.GetCultureInfo("it"),
            [Language.Japanese] = CultureInfo.GetCultureInfo("ja"),
            [Language.Spanish] = CultureInfo.GetCultureInfo("es-ES"),
            [Language.SChinese] = CultureInfo.GetCultureInfo("zh-Hans"),
            [Language.TChinese] = CultureInfo.GetCultureInfo("zh-Hant"),
            [Language.Irish] = CultureInfo.GetCultureInfo("ga"),
        };
    }
}
