using System;
using CSharpPoet;
using Impostor.Api.Innersloth.Generator.Generators;
using Microsoft.CodeAnalysis;

namespace Impostor.Api.Innersloth.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class SourceGenerator : IIncrementalGenerator
{
    private record struct Options(string ProjectDirectory);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var optionsProvider = context.AnalyzerConfigOptionsProvider
            .Select((analyzerConfigOptions, _) =>
            {
                if (!analyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDirectory))
                {
                    throw new Exception("Couldn't get project directory");
                }

                return new Options(projectDirectory);
            });

        var filesProvider = context.AdditionalTextsProvider.Combine(optionsProvider)
            .Where(static pair =>
            {
                var (file, options) = pair;
                return file.Path.StartsWith(options.ProjectDirectory) &&
                       file.Path[options.ProjectDirectory.Length..].StartsWith("Innersloth/Data/") &&
                       file.Path.EndsWith(".json");
            })
            .Select(static (pair, cancellationToken) =>
            {
                var (file, options) = pair;
                return (
                    RelativePath: file.Path[(options.ProjectDirectory.Length + "Innersloth/Data/".Length)..],
                    Content: file.GetText(cancellationToken)!.ToString()
                );
            })
            .Collect();

        context.RegisterSourceOutput(filesProvider, (spc, files) =>
        {
            var enumGenerator = new EnumGenerator(spc, files);

            enumGenerator.Generate("ColorType", "Impostor.Api.Innersloth.Customization");

            enumGenerator.Generate("DisconnectReason", sourceName: "DisconnectReasons");
            enumGenerator.Generate("GameKeywords", flags: true, underlyingType: CSharpEnumUnderlyingType.UnsignedInt);
            enumGenerator.Generate("GameOverReason", underlyingType: CSharpEnumUnderlyingType.Byte);
            enumGenerator.Generate("Platforms");
            enumGenerator.Generate("RoleTypes", underlyingType: CSharpEnumUnderlyingType.UnsignedShort);
            enumGenerator.Generate("StringNames");
            enumGenerator.Generate("SystemTypes", underlyingType: CSharpEnumUnderlyingType.Byte);
            enumGenerator.Generate("Language", sourceName: "SupportedLangs");
            enumGenerator.Generate("TaskTypes");

            enumGenerator.Generate("RpcCalls", "Impostor.Api.Net.Inner", underlyingType: CSharpEnumUnderlyingType.Byte);

            var mapDataGenerator = new MapDataGenerator(spc, files);

            var mapNames = new[] { "Skeld", "Mira", "April", "Polus", "Airship", "Fungle" };
            foreach (var mapName in mapNames)
            {
                mapDataGenerator.Generate(mapName);
            }
        });
    }
}
