using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Api.Plugins;

public interface IPlugin : IEventListener
{
    ValueTask EnableAsync()
    {
        return default;
    }

    ValueTask DisableAsync()
    {
        return default;
    }

    ValueTask ReloadAsync()
    {
        return default;
    }
}
