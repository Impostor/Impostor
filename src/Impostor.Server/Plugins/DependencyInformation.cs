using System;
using System.Reflection;
using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins
{
    public class DependencyInformation
    {
        private readonly ImpostorDependencyAttribute _attribute;

        public DependencyInformation(Type dependencyType)
        {
            _attribute = dependencyType.GetCustomAttribute<ImpostorDependencyAttribute>()!;
        }

        public string Name => _attribute.Name;

        public DependencyType DependencyType => _attribute.DependencyType;
    }
}
