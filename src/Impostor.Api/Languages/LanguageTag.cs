using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Languages;

[AttributeUsage(AttributeTargets.Field)]
public class LanguageTag(SupportedLanguages language = SupportedLanguages.English) : Attribute
{
    public LanguageTag(string key) : this()
    {
        this.Key = key;
    }

    public string? Key { get; private set; }

    public string Value { get; private set; }

    public SupportedLanguages _language = language;

    public LanguageTag Set(string key, string value)
    {
        Key = key;
        Value = value;
        return this;
    }

    public static List<LanguageTag> GetTagsFormAssembly(Assembly assembly)
    {
        var fieldInfos = new List<FieldInfo>();
        foreach (var fields in assembly.GetTypes().Select(n => n.GetFields()))
        {
            fieldInfos.AddRange(fields);
        }

        return (from fieldInfo in fieldInfos.Where(n => n.IsStatic && n.GetCustomAttribute<LanguageTag>() != null && n.FieldType == typeof(string))
            let languageTag = fieldInfo.GetCustomAttribute<LanguageTag>()!
            let key = languageTag.Key ?? fieldInfo.Name
            let value = fieldInfo.GetValue(null) as string
            select languageTag.Set(key, value!)).ToList();
    }
}
