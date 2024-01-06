using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using CSharpPoet;
using Microsoft.CodeAnalysis;

namespace Impostor.Api.Innersloth.Generator.Generators;

public sealed class MapDataGenerator : BaseGenerator
{
    public MapDataGenerator(SourceProductionContext sourceProductionContext, ImmutableArray<(string RelativePath, string Content)> files) : base(sourceProductionContext, files)
    {
    }

    private T? Deserialize<T>(string name, string fileName)
    {
        return JsonSerializer.Deserialize<T>(
            GetFileContent($"maps/{name}/{fileName}.json"),
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new Vector2Converter(),
                },
            }
        );
    }

    public void Generate(string name)
    {
        var className = name + "Data";

        var spawnInfo = Deserialize<SpawnInfo>(name, "spawn")!;
        var tasks = Deserialize<Dictionary<int, TaskInfo>>(name, "tasks")!;
        var vents = Deserialize<Dictionary<int, VentInfo>>(name, "vents")!;
        var doors = Deserialize<Dictionary<int, DoorInfo>>(name, "doors")!;

        var ventsData = new DictionaryData("VentData", "Vents", writer =>
        {
            foreach (var pair in vents)
            {
                var id = pair.Key;
                var vent = pair.Value;

                var connections = "";
                if (vent.Left != null) connections += ", left: " + vent.Left.Value;
                if (vent.Center != null) connections += ", center: " + vent.Center.Value;
                if (vent.Right != null) connections += ", right: " + vent.Right.Value;

                writer.WriteLine($"[{id}] = new(this, {id}, \"{vent.Name}\", {vent.Position.ToCSharpString()}{connections}),");
            }
        });

        var tasksData = new DictionaryData("TaskData", "Tasks", writer =>
        {
            foreach (var pair in tasks)
            {
                var id = pair.Key;
                var task = pair.Value;

                writer.WriteLine($"[{id}] = new({id}, TaskTypes.{task.TaskType}, TaskCategories.{task.Length}Task),");
            }
        });

        var doorsData = new DictionaryData("DoorData", "Doors", writer =>
        {
            foreach (var pair in doors)
            {
                var id = pair.Key;
                var door = pair.Value;

                writer.WriteLine($"[{id}] = new({id}, SystemTypes.{door.Room}, {door.Position.ToCSharpString()}),");
            }
        });

        var constructor = new CSharpMethod(Visibility.Internal, className, ".ctor")
        {
            Body = writer =>
            {
                ventsData.WriteInitializer(writer);
                writer.WriteLine();
                tasksData.WriteInitializer(writer);
                writer.WriteLine();
                doorsData.WriteInitializer(writer);
            },
        };

        var @class = new CSharpClass(className)
        {
            IsSealed = true,
            Extends = { "MapData" },
            Members =
            {
                constructor,

                ventsData.CreateProperty(),
                tasksData.CreateProperty(),
                doorsData.CreateProperty(),

                new CSharpBlankLine(),

                CreateSimpleProperty("float", "SpawnRadius", spawnInfo.SpawnRadius.ToString(CultureInfo.InvariantCulture) + "f"),
                CreateSimpleProperty("Vector2", "InitialSpawnCenter", spawnInfo.InitialSpawnCenter.ToCSharpString()),
                CreateSimpleProperty("Vector2", "MeetingSpawnCenter", spawnInfo.MeetingSpawnCenter.ToCSharpString()),
                CreateSimpleProperty("Vector2", "MeetingSpawnCenter2", spawnInfo.MeetingSpawnCenter2.ToCSharpString()),
            },
        };

        var source = new CSharpFile("Impostor.Api.Innersloth.Maps")
        {
            Usings = { "System.Collections.Generic", "System.Numerics" },
            Members = { @class },
        }.ToString();
        _sourceProductionContext.AddSource(className, source);
    }

    private static CSharpProperty CreateSimpleProperty(string type, string name, string value)
    {
        return new CSharpProperty(type, name)
        {
            Getter = new CSharpProperty.Accessor { Body = writer => writer.Write(value + ";") },
            IsOverride = true,
        };
    }

    private sealed class DictionaryData
    {
        private readonly string _keyType;
        private readonly string _valueType;
        private readonly string _name;
        private readonly Action<CodeWriter> _body;

        public DictionaryData(string keyType, string valueType, string name, Action<CodeWriter> body)
        {
            _keyType = keyType;
            _valueType = valueType;
            _name = name;
            _body = body;
        }

        public DictionaryData(string valueType, string name, Action<CodeWriter> body) : this("int", valueType, name, body)
        {
        }

        public void WriteInitializer(CodeWriter writer)
        {
            writer.WriteLine($"{_name} = new Dictionary<{_keyType}, {_valueType}>");
            writer.WriteLine("{");

            using (writer.Indent())
            {
                _body(writer);
            }

            writer.WriteLine("}.AsReadOnly();");
        }

        public CSharpProperty CreateProperty()
        {
            return new CSharpProperty($"IReadOnlyDictionary<{_keyType}, {_valueType}>", _name)
            {
                Getter = new CSharpProperty.Accessor(),
                IsOverride = true,
            };
        }
    }

    public sealed class SpawnInfo
    {
        public required Vector2 InitialSpawnCenter { get; init; }
        public required Vector2 MeetingSpawnCenter { get; init; }
        public required Vector2 MeetingSpawnCenter2 { get; init; }
        public required float SpawnRadius { get; init; }
    }

    public sealed class TaskInfo
    {
        public required string Type { get; init; }
        public required string TaskType { get; init; }
        public required string Length { get; init; }
        public required TaskConsole[] Consoles { get; init; }
    }

    public sealed class TaskConsole
    {
        public required int Id { get; init; }
        public required string Room { get; init; }
        public required Vector2 Position { get; init; }
        public required float UsableDistance { get; init; }
    }

    public sealed class VentInfo
    {
        public required string Name { get; init; }
        public required Vector2 Position { get; init; }
        public required int? Left { get; init; }
        public required int? Center { get; init; }
        public required int? Right { get; init; }
    }

    public sealed class DoorInfo
    {
        public required string Room { get; init; }
        public required Vector2 Position { get; init; }
    }
}
