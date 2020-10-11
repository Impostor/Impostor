using System.Threading.Tasks;
using Impostor.Server.Events;

namespace Impostor.Server.Plugins
{
    public interface IPlugin : IEventListener
    {
        ValueTask EnableAsync();

        ValueTask DisableAsync();

        ValueTask ReloadAsync();
    }
}