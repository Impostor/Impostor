using System;

namespace Impostor.Api.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImpostorDependencyAttribute : Attribute
    {
        public ImpostorDependencyAttribute(string name, DependencyType type)
        {
            Name = name;
            DependencyType = type;
        }

        public DependencyType DependencyType { get; }

        public string Name { get; }
    }
}
