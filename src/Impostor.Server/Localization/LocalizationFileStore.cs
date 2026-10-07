using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Impostor.Api.Innersloth;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Localization
{
    internal sealed class LocalizationFileStore
    {
        private readonly ILogger _logger;
        private readonly string _directory;

        public LocalizationFileStore(string contentRoot, ILogger logger)
        {
            _directory = Path.Combine(contentRoot, "languages");
            _logger = logger;
        }

        public CatalogState LoadOverrides(CatalogState state)
        {
            var tables = new Dictionary<Language, IReadOnlyDictionary<string, string>>();
            var fallback = state.Defaults.Translations[state.Defaults.DefaultLanguage];
            foreach (var (language, code) in LanguageCodes.All)
            {
                var file = Path.Combine(_directory, state.Defaults.Owner, code + ".json");
                if (!File.Exists(file) || !TryRead(file, out var table))
                {
                    continue;
                }

                var valid = new Dictionary<string, string>();
                foreach (var (key, text) in table!)
                {
                    if (!fallback.TryGetValue(key, out var defaultText) || !LocalizationResourceLoader.ValidArguments(text, defaultText))
                    {
                        _logger.LogWarning("Unknown localization key or invalid arguments for {Key} in {File}; using defaults", key, file);
                        continue;
                    }

                    valid.Add(key, text);
                }

                tables.Add(language, valid);
            }

            return state with { Overrides = tables };
        }

        public void EnsureFiles(LoadedCatalog catalog)
        {
            foreach (var (language, values) in catalog.Translations)
            {
                var file = Path.Combine(_directory, catalog.Owner, LanguageCodes.All[language] + ".json");
                Dictionary<string, string> existing;
                if (File.Exists(file))
                {
                    if (!TryRead(file, out var parsed))
                    {
                        continue;
                    }

                    existing = parsed!;
                }
                else
                {
                    existing = new Dictionary<string, string>();
                }

                var changed = false;
                foreach (var (key, text) in values)
                {
                    changed |= existing.TryAdd(key, text);
                }

                if (changed)
                {
                    TryWrite(file, JsonSerializer.Serialize(existing, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    }));
                }
            }
        }

        private bool TryRead(string file, out Dictionary<string, string>? table)
        {
            table = null;
            string json;
            try
            {
                json = File.ReadAllText(file);
            }
            catch (IOException ex)
            {
                _logger.LogWarning("Cannot read localization file {File}: {Reason}", file, ex.Message);
                return false;
            }

            return LocalizationResourceLoader.TryParse(json, file, _logger, out table);
        }

        private void TryWrite(string file, string json)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                File.WriteAllText(file, json);
            }
            catch (IOException ex)
            {
                _logger.LogWarning("Cannot write localization file {File}: {Reason}", file, ex.Message);
            }
        }
    }
}
