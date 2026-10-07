using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Impostor.Api.Innersloth;
using Impostor.Api.Localization;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Localization
{
    internal static class LocalizationResourceLoader
    {
        public static bool TryLoad(LocalizationCatalog catalog, ILogger logger, out LoadedCatalog? loaded)
        {
            loaded = null;
            if (catalog == null || string.IsNullOrWhiteSpace(catalog.Owner) || catalog.Owner.Length > 128
                || catalog.Owner is "." or ".." || catalog.Owner.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '.' or '_' or '-'))
                || catalog.Assembly == null || string.IsNullOrWhiteSpace(catalog.ResourcePrefix))
            {
                logger.LogWarning("Invalid localization catalog owner, assembly or resource prefix");
                return false;
            }

            var translations = new Dictionary<Language, IReadOnlyDictionary<string, string>>();
            var names = catalog.Assembly.GetManifestResourceNames().ToHashSet(StringComparer.Ordinal);
            foreach (var (language, code) in LanguageCodes.All)
            {
                var name = $"{catalog.ResourcePrefix}.{code}.json";
                if (!names.Contains(name))
                {
                    continue;
                }

                using var stream = catalog.Assembly.GetManifestResourceStream(name);
                if (stream == null)
                {
                    logger.LogWarning("Missing localization resource {Resource} for {Owner}", name, catalog.Owner);
                    return false;
                }

                using var reader = new StreamReader(stream);
                if (!TryParse(reader.ReadToEnd(), name, logger, out var table))
                {
                    return false;
                }

                translations.Add(language, table!);
            }

            if (!LanguageCodes.All.ContainsKey(catalog.DefaultLanguage) || !translations.TryGetValue(catalog.DefaultLanguage, out var fallback) || fallback.Count == 0)
            {
                logger.LogWarning("Catalog {Owner} needs a supported, nonempty default language JSON resource", catalog.Owner);
                return false;
            }

            foreach (var table in translations.Values)
            {
                foreach (var (key, value) in table)
                {
                    if (!fallback.TryGetValue(key, out var defaultText) || !ValidArguments(value, defaultText))
                    {
                        logger.LogWarning("Invalid localization key or arguments for {Owner}:{Key}", catalog.Owner, key);
                        return false;
                    }
                }
            }

            loaded = new LoadedCatalog(catalog.Owner, catalog.DefaultLanguage, translations);
            return true;
        }

        public static bool TryParse(string json, string source, ILogger logger, out Dictionary<string, string>? table)
        {
            table = null;
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                logger.LogWarning("Invalid localization JSON in {Source}: {Reason}", source, ex.Message);
                return false;
            }

            using (document)
            {
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    logger.LogWarning("Localization JSON in {Source} must be an object", source);
                    return false;
                }

                var values = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var property in document.RootElement.EnumerateObject())
                {
                    if (string.IsNullOrWhiteSpace(property.Name) || property.Value.ValueKind != JsonValueKind.String)
                    {
                        logger.LogWarning("Invalid localization value for {Key} in {Source}", property.Name, source);
                        return false;
                    }

                    var text = property.Value.GetString()!;
                    if (!TryFormat(text, out _) || !values.TryAdd(property.Name, text))
                    {
                        logger.LogWarning("Duplicate key or invalid format for {Key} in {Source}", property.Name, source);
                        return false;
                    }
                }

                table = values;
                return true;
            }
        }

        public static bool TryFormat(string text, out CompositeFormat? format)
        {
            try
            {
                format = CompositeFormat.Parse(text);
                return true;
            }
            catch (FormatException)
            {
                format = null;
                return false;
            }
        }

        public static bool ValidArguments(string text, string defaultText) =>
            TryFormat(text, out var format) && TryFormat(defaultText, out var fallback)
            && format!.MinimumArgumentCount <= fallback!.MinimumArgumentCount;
    }
}
