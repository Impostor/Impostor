using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins
{
    public class DependencyInformation
    {
        private readonly ImpostorDependencyAttribute _attribute;

        public DependencyInformation(ImpostorDependencyAttribute attribute)
        {
            _attribute = attribute;
        }

        public string Id => _attribute.Id;

        public DependencyType DependencyType => _attribute.DependencyType;
    }
}
