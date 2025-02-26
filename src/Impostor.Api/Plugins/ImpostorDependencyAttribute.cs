using System;

namespace Impostor.Api.Plugins;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ImpostorDependencyAttribute(string id, DependencyType type) : Attribute
{
    public string Id { get; } = id;

    public DependencyType DependencyType { get; } = type;
}
