using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Api.Plugins
{
    public interface IPlugin : IEventListener
    {
        ValueTask EnableAsync();

        ValueTask DisableAsync();

        ValueTask ReloadAsync();
    }
}
