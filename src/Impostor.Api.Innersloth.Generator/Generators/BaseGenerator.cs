using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Impostor.Api.Innersloth.Generator.Generators;

public abstract class BaseGenerator
{
    protected ImmutableArray<(string RelativePath, string Content)> _files;
    protected SourceProductionContext _sourceProductionContext;

    protected BaseGenerator(SourceProductionContext sourceProductionContext,
        ImmutableArray<(string RelativePath, string Content)> files)
    {
        _sourceProductionContext = sourceProductionContext;
        _files = files;
    }

    protected string GetFileContent(string path)
    {
        return _files.Single(x => x.RelativePath == path).Content;
    }
}
