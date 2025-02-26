using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Impostor.Api.Generator.Generators;

public class BaseGenerator(
    SourceProductionContext sourceProductionContext,
    ImmutableArray<(string RelativePath, string Content)> files)
{
    internal ImmutableArray<(string RelativePath, string Content)> _files = files;
    internal SourceProductionContext _sourceProductionContext = sourceProductionContext;

    protected string GetFileContent(string path)
    {
        return _files.Single(x => x.RelativePath == path).Content;
    }
}

public static class GeneratorExtensions
{
    private static readonly List<BaseGenerator> Generators = [];

    private static bool TryGetGenerator<T>([MaybeNullWhen(false)]out T generator) where T : BaseGenerator
    {
        if (Generators.FirstOrDefault(x => x is T) is T get)
        {
            generator = get;
            return true;
        }

        generator = null;
        return false;
    }
    
    public static EnumGenerator GetEnum(this BaseGenerator @base)
    {
        if (TryGetGenerator<EnumGenerator>(out var generator))
            return generator;
        
        var newGenerator = new EnumGenerator(@base._sourceProductionContext, @base._files);
        Generators.Add(newGenerator);
        return newGenerator;
    }

    public static MapDataGenerator GetMapData(this BaseGenerator @base)
    {
        if (TryGetGenerator<MapDataGenerator>(out var generator))
            return generator;
        
        var newGenerator = new MapDataGenerator(@base._sourceProductionContext, @base._files);
        Generators.Add(newGenerator);
        return newGenerator;
    }

    /*public static LanguageGenerator GetLanguage(this BaseGenerator @base)
    {
        if (TryGetGenerator<LanguageGenerator>(out var generator))
            return generator;
        
        var newGenerator = new LanguageGenerator(@base._sourceProductionContext, @base._files);
        Generators.Add(newGenerator);
        return newGenerator;
    }*/
}
