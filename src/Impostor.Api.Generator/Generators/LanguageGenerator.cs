using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using CSharpPoet;
using Microsoft.CodeAnalysis;

namespace Impostor.Api.Generator.Generators;

/*public class LanguageGenerator(SourceProductionContext sourceProductionContext, ImmutableArray<(string RelativePath, string Content)> files) 
    : BaseGenerator(sourceProductionContext, files)
{
    private List<string> _keys = null!;
    private const string NameSpace = "Impostor.Api.Languages";
    private static readonly CSharpUsing LanguageTypeUsing = new("Impostor.Api.Innersloth");
    
    public void Generate(params List<string> languageNames)
    {
        if (!GenerateLanguageInterface())
            return;
        
        foreach (var language in languageNames)
        {
            var isBase = language == "English";
            var dic = JsonSerializer.Deserialize<Dictionary<string, string>>(GetFileContent($"{language}.json"));
            if (dic is null)
            {
                if (isBase)
                {
                    break;
                }
                continue;
            }

            var fields = GenerateFields(isBase, dic, language);
            var languageFile = new CSharpFile("Impostor.Api.Languages")
            {
                Usings = 
                new CSharpClass(Visibility.Public, language)
                {
                    Extends = isBase ? [] : ["English"],
                    Members = fields,
                },
            };
            languageFile.Usings.Add(new CSharpUsing("Impostor.Api.Innersloth"));
            
            _sourceProductionContext.AddSource(language, languageFile.ToString());
        }
    }

    private bool GenerateLanguageInterface()
    {
        try
        {
            var keys = JsonSerializer.Deserialize<List<string>>(GetFileContent("keys.json"));
            if (keys is null)
                return false;
            
            _keys = keys;

            var fields = keys.Select(CSharpType.IMember (n) => new CSharpField(Visibility.Public, "string", n + " { get; set; }"));
            var file = new CSharpFile(NameSpace)
            {
                Usings = [LanguageTypeUsing],
                Members = [
                    new CSharpInterface(Visibility.Public, "ILanguage") 
                    {
                        Members = fields,
                    }],
            };
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private void GenerateLanguage(string language)
    {
        
    }

    private static List<CSharpType.IMember> GenerateFields(bool isBase, Dictionary<string, string> dic, string langName)
    {
        var members = dic.Select(CSharpType.IMember (pair) =>
        {
            var field = new CSharpField(Visibility.Public, "string", pair.Key + " { get; set; }")
            {
                DefaultValue = $"\"{pair.Value}\"",
            };
            field.Modifiers |= isBase ? Modifiers.Virtual : Modifiers.Override;
            return field;
        }).ToList();
        members.Add(new CSharpField(Visibility.Public, "Language", "LangType { get; set; }")
        {
            Modifiers = isBase ? Modifiers.Virtual : Modifiers.Override,
            DefaultValue = $"Language.{langName}",
        });
        
        return members;
    }
}*/
