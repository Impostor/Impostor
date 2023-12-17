using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using CSharpPoet;
using Microsoft.CodeAnalysis;

namespace Impostor.Api.Innersloth.Generator.Generators;

public sealed class EnumGenerator : BaseGenerator
{
    public EnumGenerator(SourceProductionContext sourceProductionContext, ImmutableArray<(string RelativePath, string Content)> files) : base(sourceProductionContext, files)
    {
    }

    public void Generate(string name, string? @namespace = null, string? sourceName = null, bool flags = false, CSharpEnumUnderlyingType underlyingType = CSharpEnumUnderlyingType.Int)
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, long>>(GetFileContent($"enums/{sourceName ?? name}.json"))!;

        var @enum = new CSharpEnum(name, underlyingType);

        foreach (var pair in dictionary)
        {
            var value = flags && pair.Value > 0
                ? $"1 << {Math.Log(pair.Value, 2)}"
                : pair.Value.ToString();

            @enum.Members.Add(new CSharpEnum.Member(pair.Key, value));
        }

        if (flags)
        {
            @enum.Attributes.Add(new CSharpAttribute("System.FlagsAttribute"));
        }

        var source = new CSharpFile(@namespace ?? "Impostor.Api.Innersloth") { @enum }.ToString();
        _sourceProductionContext.AddSource(name, source);
    }
}
