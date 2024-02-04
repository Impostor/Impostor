using System;
using System.Collections.Generic;
using System.IO;
using Impostor.Api.Innersloth;
using Impostor.Api.Languages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server;

public class LanguageManager : ILanguageManager
{
    private List<LanguageTag> _languageTags;

    public LanguageManager(List<LanguageTag> languageTags)
    {
        _languageTags = languageTags;
        LanguageExtension.SetLanguageManager(this);
    }

    public string GetString(string key)
    {
        return string.Empty;
    }

    public void AddTranslate(string key, string value)
    {
    }

    public void AddTranslate(LanguageTag tag)
    {
    }

    public void RemoveTranslate(string key)
    {
    }

    public void RemoveTranslate(LanguageTag tag)
    {
    }
}

#pragma warning disable SA1402
#pragma warning disable SA1204
public static class LanguageLoader
#pragma warning restore SA1204
#pragma warning restore SA1402
{
    public static IHostBuilder UseLanguage(this IHostBuilder builder)
    {
        var rootDir = Directory.GetCurrentDirectory();
        var langDir = Path.Combine(rootDir, "Language");
        var translates = new List<LanguageTag>();

        translates.AddRange(LanguageTag.GetTagsFormAssembly(typeof(LanguageTag).Assembly));
        translates.AddRange(LanguageTag.GetTagsFormAssembly(typeof(LanguageManager).Assembly));

        if (!Directory.Exists(langDir))
        {
            Directory.CreateDirectory(langDir);
        }

        foreach (var varLang in Enum.GetNames(typeof(SupportedLanguages)))
        {
            var fileName = varLang.Replace("SupportedLanguages.", string.Empty) + ".txt";
            var path = Path.Combine(langDir, fileName);

            if (!File.Exists(path))
            {
                continue;
            }

            using TextReader reader = new StreamReader(File.OpenRead(path));
            translates.AddRange(Serialize(reader, Enum.Parse<SupportedLanguages>(varLang)));
        }

        builder.ConfigureServices((host, services) =>
        {
            services.AddSingleton<LanguageManager>(provider =>
                ActivatorUtilities.CreateInstance<LanguageManager>(provider, translates));
        });

        return builder;
    }

    private static IEnumerable<LanguageTag> Serialize(TextReader reader, SupportedLanguages language)
    {
        var list = new List<LanguageTag>();
        while (reader.ReadLine() is { } line)
        {
            if (!line.Contains(':'))
            {
                continue;
            }

            var dic = line.Split(":");
            list.Add(new LanguageTag(language).Set(dic[0], dic[1]));
        }

        return list;
    }
}
