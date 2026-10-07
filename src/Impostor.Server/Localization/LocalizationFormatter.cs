using System;
using System.Collections.Generic;
using System.Globalization;
using Impostor.Api.Innersloth;
using Impostor.Api.Localization;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Localization
{
    internal static class LocalizationFormatter
    {
        public static string Get(CatalogState? state, LocalizedMessageKey message, Language language, object?[] arguments, ILogger logger)
        {
            if (state != null && message.Key != null)
            {
                var selected = LanguageCodes.All.ContainsKey(language) ? language : state.Defaults.DefaultLanguage;
                var culture = CultureInfo.GetCultureInfo(LanguageCodes.All[selected]);
                foreach (var candidate in Candidates(state, message.Key, selected))
                {
                    if (!LocalizationResourceLoader.TryFormat(candidate, out var format) || arguments.Length < format!.MinimumArgumentCount)
                    {
                        logger.LogWarning("Missing arguments or invalid format for localization message {Message}", message);
                        continue;
                    }

                    try
                    {
                        return string.Format(culture, candidate, arguments);
                    }
                    catch (FormatException ex)
                    {
                        logger.LogWarning(ex, "Cannot format localization message {Message}", message);
                    }
                }
            }

            logger.LogWarning("Missing localization message or arguments for {Message}", message);
            return message.ToString();
        }

        private static IEnumerable<string> Candidates(CatalogState state, string key, Language language)
        {
            if (state.Overrides.TryGetValue(language, out var overrides) && overrides.TryGetValue(key, out var text))
            {
                yield return text;
            }

            if (state.Defaults.Translations.TryGetValue(language, out var defaults) && defaults.TryGetValue(key, out text))
            {
                yield return text;
            }

            if (language != state.Defaults.DefaultLanguage)
            {
                foreach (var fallback in Candidates(state, key, state.Defaults.DefaultLanguage))
                {
                    yield return fallback;
                }
            }
        }
    }
}
