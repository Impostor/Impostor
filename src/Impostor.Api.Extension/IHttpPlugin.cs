using Impostor.Api.Plugins;

namespace Impostor.Api.Extension;

public interface IHttpPlugin : IPlugin
{
    bool AssemblyPart
    {
        get => false;
    }
}
