using System;

namespace Impostor.Api.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ImpostorDependencyAttribute : Attribute
    {
        public ImpostorDependencyAttribute(string name, DependencyType type)
        {
            Name = name;
            DependencyType = type;
        }

        public string Name { get; }

        public DependencyType DependencyType { get; }
    }
}
