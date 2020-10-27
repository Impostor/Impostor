using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Server.Plugins;

namespace Impostor.Api.Plugins.Managers
{
    public interface IPluginManager
    {
        IReadOnlyList<IPluginInformation> Plugins { get; }

        /// <summary>
        ///     Reload plugin host.
        /// </summary>
        /// <param name="reloadAssemblies">True if the assemblies should be reloaded.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask ReloadAsync(bool reloadAssemblies = false);
    }
}
