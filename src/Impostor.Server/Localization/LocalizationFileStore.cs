using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Impostor.Api.Innersloth;
using Impostor.Api.Localization;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Localization
{
    internal sealed class LocalizationFileStore
    {
        private static readonly JsonSerializerOptions OverrideFileOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

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
            foreach (var (language, culture) in LanguageCodes.All)
            {
                var file = Path.Combine(_directory, state.Defaults.Owner, culture.Name + ".json");
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

        public void SyncOverrideFiles(LoadedCatalog catalog)
        {
            foreach (var (language, values) in catalog.Translations)
            {
                var culture = LanguageCodes.All[language];
                var file = Path.Combine(_directory, catalog.Owner, culture.Name + ".json");
                var defaultsFile = Path.Combine(_directory, catalog.Owner, culture.Name + ".defaults.json");
                var created = !File.Exists(file);

                if (!TryReadTable(defaultsFile, out var previous) || !TryReadTable(file, out var current))
                {
                    continue;
                }

                var (added, updated, removed, kept) = MergeDefaults(values, previous, current, file);

                if (added.Count + updated.Count + removed.Count > 0)
                {
                    TryWrite(file, JsonSerializer.Serialize(current, OverrideFileOptions));
                }

                if (added.Count + updated.Count + removed.Count + kept.Count > 0)
                {
                    Report(file, created, added, updated, removed, kept, current.Count);
                }

                if (!SameContent(previous, values))
                {
                    TryWrite(defaultsFile, JsonSerializer.Serialize(values, OverrideFileOptions));
                }
            }
        }

        private static bool SameContent(IReadOnlyDictionary<string, string> left, IReadOnlyDictionary<string, string> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            foreach (var (key, value) in right)
            {
                if (!left.TryGetValue(key, out var other) || other != value)
                {
                    return false;
                }
            }

            return true;
        }

        private static string Unit(int count) => count == 1 ? "localization key" : "localization keys";

        private void Report(string file, bool created, IReadOnlyCollection<string> added, IReadOnlyCollection<string> updated, IReadOnlyCollection<string> removed, IReadOnlyCollection<string> kept, int total)
        {
            if (created)
            {
                _logger.LogInformation("Created {File} with {Count} {Unit}", file, total, Unit(total));
                return;
            }

            if (added.Count > 0)
            {
                _logger.LogInformation("Added {Count} {Unit} to {File}: {Keys}", added.Count, Unit(added.Count), file, string.Join(", ", added));
            }

            if (updated.Count > 0)
            {
                _logger.LogInformation("Updated {Count} {Unit} in {File} from the catalog defaults: {Keys}", updated.Count, Unit(updated.Count), file, string.Join(", ", updated));
            }

            if (removed.Count > 0)
            {
                _logger.LogInformation("Removed {Count} obsolete {Unit} from {File}: {Keys}", removed.Count, Unit(removed.Count), file, string.Join(", ", removed));
            }

            if (kept.Count > 0)
            {
                _logger.LogWarning(
                    "Kept the customized text of {Count} {Unit} in {File} although the catalog default changed; review whether the text is still up to date: {Keys}",
                    kept.Count,
                    Unit(kept.Count),
                    file,
                    string.Join(", ", kept));
            }
        }

        private (List<string> Added, List<string> Updated, List<string> Removed, List<string> Kept) MergeDefaults(IReadOnlyDictionary<string, string> defaults, IReadOnlyDictionary<string, string> previous, Dictionary<string, string> current, string file)
        {
            var added = new List<string>();
            var updated = new List<string>();
            var removed = new List<string>();
            var kept = new List<string>();

            foreach (var (key, text) in defaults)
            {
                if (!current.TryGetValue(key, out var value))
                {
                    current[key] = text;
                    added.Add(key);
                    continue;
                }

                if (!previous.TryGetValue(key, out var oldDefault))
                {
                    continue;
                }

                if (value == oldDefault)
                {
                    // Still exactly the text we generated last time, so nobody customized it.
                    if (value != text)
                    {
                        current[key] = text;
                        updated.Add(key);
                    }

                    continue;
                }

                if (oldDefault != text)
                {
                    // Customized, but the catalog text changed as well: keep the override and ask for a review.
                    kept.Add(key);
                }
            }

            foreach (var (key, oldDefault) in previous)
            {
                if (defaults.ContainsKey(key) || !current.TryGetValue(key, out var value))
                {
                    continue;
                }

                if (value == oldDefault)
                {
                    current.Remove(key);
                    removed.Add(key);
                }
                else
                {
                    _logger.LogWarning("Localization key {Key} in {File} is no longer used by the catalog; remove it manually", key, file);
                }
            }

            return (added, updated, removed, kept);
        }

        private bool TryReadTable(string file, out Dictionary<string, string> table)
        {
            table = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!File.Exists(file))
            {
                return true;
            }

            if (!TryRead(file, out var parsed))
            {
                return false;
            }

            table = parsed!;
            return true;
        }

        private bool TryRead(string file, out Dictionary<string, string>? table)
        {
            table = null;
            string json;
            try
            {
                json = File.ReadAllText(file);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
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
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _logger.LogWarning("Cannot write localization file {File}: {Reason}", file, ex.Message);
            }
        }
    }
}
