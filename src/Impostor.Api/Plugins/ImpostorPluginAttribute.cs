using System;

namespace Impostor.Api.Plugins;

[AttributeUsage(AttributeTargets.Class)]
public class ImpostorPluginAttribute(string id) : Attribute
{
    [Obsolete("Use (string id) constructor to avoid redundancy")]
    public ImpostorPluginAttribute(string id, string name, string author, string version) : this(id)
    {
        Name = name;
        Author = author;
        Version = version;
    }

    public string Id { get; } = id;

    public string? Name { get; }

    public string? Author { get; }

    public string? Version { get; }
}
