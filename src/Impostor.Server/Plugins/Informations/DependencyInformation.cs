using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins.Informations;

public class DependencyInformation(ImpostorDependencyAttribute attribute)
{
    public string Id
    {
        get => attribute.Id;
    }

    public DependencyType DependencyType
    {
        get => attribute.DependencyType;
    }
}
