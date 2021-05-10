using System;

namespace Impostor.Api.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ImpostorDependencyAttribute : Attribute
    {
        public ImpostorDependencyAttribute(string id, DependencyType type)
        {
            Id = id;
            DependencyType = type;
        }

        public string Id { get; }

        public DependencyType DependencyType { get; }
    }
}
