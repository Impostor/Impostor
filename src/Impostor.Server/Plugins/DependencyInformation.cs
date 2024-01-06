using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins;

public class DependencyInformation(ImpostorDependencyAttribute attribute)
{
    public string Id => attribute.Id;

    public DependencyType DependencyType => attribute.DependencyType;
}
