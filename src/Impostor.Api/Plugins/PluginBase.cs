using System.Threading.Tasks;

namespace Impostor.Api.Plugins
{
    public class PluginBase : IPlugin
    {
        public virtual ValueTask EnableAsync()
        {
            return default;
        }

        public virtual ValueTask DisableAsync()
        {
            return default;
        }

        public virtual ValueTask ReloadAsync()
        {
            return default;
        }
    }
}
